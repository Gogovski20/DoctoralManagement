import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AllDoctoralProjectsPage() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filterStatus, setFilterStatus] = useState('');

  useEffect(() => {
    fetchProjects();
  }, []);

  const fetchProjects = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getAllDoctoralProjects();
      setProjects(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to fetch projects:', err);
      setError(err.response?.data?.message || 'Failed to load doctoral projects');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    if (!status) return '#6b7280';
    const statusStr = status.toString().toLowerCase();
    switch (statusStr) {
      case 'draft':
        return '#6b7280';
      case 'submitted':
        return '#3b82f6';
      case 'underreview':
      case 'under_review':
        return '#f59e0b';
      case 'approved':
        return '#10b981';
      case 'changesrequested':
      case 'changes_requested':
        return '#f97316';
      case 'rejected':
        return '#ef4444';
      case 'completed':
        return '#059669';
      default:
        return '#6b7280';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return 'Draft';
    return status
      .toString()
      .replace(/([A-Z])/g, ' $1')
      .trim()
      .replace(/\b\w/g, l => l.toUpperCase());
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch (e) {
      return 'Invalid Date';
    }
  };

  const canReview = (status) => {
    return status === 'Submitted' || status === 'UnderReview' || status === 'ChangesRequested';
  };

  const canComplete = (status) => {
    return status === 'Approved';
  };

  const filteredProjects = filterStatus
    ? projects.filter(p => p.status === filterStatus)
    : projects;

  const statuses = ['Draft', 'Submitted', 'UnderReview', 'Approved', 'ChangesRequested', 'Rejected', 'Completed'];

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Admin Dashboard
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            All Doctoral Projects
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Review and manage all doctoral projects in the system
          </p>
        </div>

        {error && (
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
          }}>
            {error}
          </div>
        )}

        {/* Filter */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '1rem',
          border: '1px solid #e5e7eb',
          marginBottom: '1.5rem',
        }}>
          <label style={{ color: '#6b7280', fontSize: '0.875rem', fontWeight: '500', marginRight: '0.5rem' }}>
            Filter by Status:
          </label>
          <select
            value={filterStatus}
            onChange={(e) => setFilterStatus(e.target.value)}
            style={{
              padding: '0.5rem',
              border: '1px solid #d1d5db',
              borderRadius: '0.5rem',
              fontSize: '0.875rem',
            }}
          >
            <option value="">All Statuses</option>
            {statuses.map(status => (
              <option key={status} value={status}>
                {getStatusLabel(status)}
              </option>
            ))}
          </select>
        </div>

        {/* Projects Table */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          border: '1px solid #e5e7eb',
          overflow: 'hidden',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
        }}>
          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading projects...</p>
            </div>
          ) : filteredProjects.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🔬</div>
              <p>No projects found</p>
            </div>
          ) : (
            <table style={{
              width: '100%',
              borderCollapse: 'collapse',
            }}>
              <thead>
                <tr style={{ backgroundColor: '#f9fafb', borderBottom: '1px solid #e5e7eb' }}>
                  <th style={{ padding: '1rem', textAlign: 'left', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Title</th>
                  <th style={{ padding: '1rem', textAlign: 'left', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Student</th>
                  <th style={{ padding: '1rem', textAlign: 'left', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Mentor</th>
                  <th style={{ padding: '1rem', textAlign: 'left', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Status</th>
                  <th style={{ padding: '1rem', textAlign: 'left', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Created</th>
                  <th style={{ padding: '1rem', textAlign: 'center', fontWeight: '600', color: '#1f2937', fontSize: '0.875rem' }}>Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredProjects.map((project) => (
                  <tr key={project.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                    <td style={{ padding: '1rem', color: '#1f2937', fontWeight: '500' }}>
                      <Link
                        to={`/admin/doctoral-projects/${project.id}`}
                        style={{
                          color: '#0d9488',
                          textDecoration: 'none',
                          fontWeight: '600',
                        }}
                      >
                        {project.title}
                      </Link>
                    </td>
                    <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                      {project.studentName || 'N/A'}
                    </td>
                    <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                      {project.mentorName || 'N/A'}
                    </td>
                    <td style={{ padding: '1rem' }}>
                      <span style={{
                        display: 'inline-block',
                        padding: '0.25rem 0.75rem',
                        borderRadius: '9999px',
                        backgroundColor: getStatusColor(project.status) + '20',
                        color: getStatusColor(project.status),
                        fontWeight: '500',
                        fontSize: '0.75rem',
                      }}>
                        {getStatusLabel(project.status)}
                      </span>
                    </td>
                    <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                      {formatDate(project.createdAt)}
                    </td>
                    <td style={{ padding: '1rem', textAlign: 'center' }}>
                      <div style={{ display: 'flex', gap: '0.5rem', justifyContent: 'center' }}>
                        <Link
                          to={`/admin/doctoral-projects/${project.id}`}
                          style={{
                            backgroundColor: '#0d9488',
                            color: 'white',
                            padding: '0.5rem 1rem',
                            borderRadius: '0.5rem',
                            textDecoration: 'none',
                            fontWeight: '500',
                            fontSize: '0.75rem',
                          }}
                        >
                          View
                        </Link>
                        {canReview(project.status) && (
                          <Link
                            to={`/admin/doctoral-projects/${project.id}/review`}
                            style={{
                              backgroundColor: '#f59e0b',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              textDecoration: 'none',
                              fontWeight: '500',
                              fontSize: '0.75rem',
                            }}
                          >
                            Review
                          </Link>
                        )}
                        {canComplete(project.status) && (
                          <Link
                            to={`/admin/doctoral-projects/${project.id}/complete`}
                            style={{
                              backgroundColor: '#10b981',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              textDecoration: 'none',
                              fontWeight: '500',
                              fontSize: '0.75rem',
                            }}
                          >
                            Complete
                          </Link>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
