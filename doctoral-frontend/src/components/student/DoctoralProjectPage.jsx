import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function DoctoralProjectPage() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchProjects();
  }, []);

  const fetchProjects = async () => {
    try {
      setLoading(true);
      setError('');
      console.log('Fetching projects:');
      // Use /my endpoint - backend resolves studentId from JWT
      const data = await studentService.getMyDoctoralProjects();
      console.log('Projects data:', data);
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

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/dashboard" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Dashboard
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Doctoral Projects
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Create and manage your doctoral research project
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

        {/* Projects Section */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Your Projects ({projects.length})
            </h2>
            {!loading && (
              <Link
                to="/doctoral-project/new"
                style={{
                  backgroundColor: '#0d9488',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  textDecoration: 'none',
                  fontWeight: '500',
                }}
              >
                + Create New Project
              </Link>
            )}
          </div>

          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading your projects...</p>
            </div>
          ) : projects.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🔬</div>
              <p style={{ marginBottom: '1rem' }}>No doctoral projects yet.</p>
              <Link to="/doctoral-project/new" style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                textDecoration: 'none',
                fontWeight: '500',
                display: 'inline-block',
              }}>
                Create your first project
              </Link>
            </div>
          ) : (
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '1.5rem' }}>
              {projects.map((project) => (
                <div
                  key={project.id}
                  style={{
                    border: '1px solid #e5e7eb',
                    borderRadius: '0.75rem',
                    padding: '1.5rem',
                    backgroundColor: 'white',
                    transition: 'all 0.2s',
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1)';
                    e.currentTarget.style.borderColor = '#0d9488';
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.boxShadow = 'none';
                    e.currentTarget.style.borderColor = '#e5e7eb';
                  }}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '1rem' }}>
                    <div>
                      <h3 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.5rem 0', fontSize: '1.125rem' }}>
                        {project.title}
                      </h3>
                      <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                        {project.researchArea}
                      </p>
                    </div>
                    <span style={{
                      display: 'inline-block',
                      padding: '0.25rem 0.75rem',
                      borderRadius: '9999px',
                      backgroundColor: getStatusColor(project.status) + '20',
                      color: getStatusColor(project.status),
                      fontWeight: '500',
                      fontSize: '0.75rem',
                      whiteSpace: 'nowrap',
                    }}>
                      {getStatusLabel(project.status)}
                    </span>
                  </div>

                  <div style={{ marginBottom: '1rem' }}>
                    <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                      Mentor: <span style={{ fontWeight: '500', color: '#1f2937' }}>{project.mentorName || 'N/A'}</span>
                    </p>
                    <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                      Created: <span style={{ fontWeight: '500', color: '#1f2937' }}>{formatDate(project.createdAt)}</span>
                    </p>
                  </div>

                  {project.proposalDocumentPath && (
                    <div style={{
                      backgroundColor: '#f0fdf4',
                      border: '1px solid #dcfce7',
                      padding: '0.75rem',
                      borderRadius: '0.5rem',
                      marginBottom: '1rem',
                    }}>
                      <p style={{ fontSize: '0.75rem', color: '#166534', margin: 0 }}>
                        ✓ Proposal document uploaded
                      </p>
                    </div>
                  )}

                  <div style={{ display: 'flex', gap: '0.5rem' }}>
                    <Link
                      to={`/doctoral-project/${project.id}`}
                      style={{
                        flex: 1,
                        backgroundColor: '#0d9488',
                        color: 'white',
                        padding: '0.5rem 1rem',
                        borderRadius: '0.5rem',
                        textDecoration: 'none',
                        fontWeight: '500',
                        fontSize: '0.875rem',
                        textAlign: 'center',
                      }}
                    >
                      View
                    </Link>
                    {(project.status === 'Draft' || project.status === 'ChangesRequested') && (
                      <Link
                        to={`/doctoral-project/${project.id}/upload`}
                        style={{
                          flex: 1,
                          backgroundColor: '#3b82f6',
                          color: 'white',
                          padding: '0.5rem 1rem',
                          borderRadius: '0.5rem',
                          textDecoration: 'none',
                          fontWeight: '500',
                          fontSize: '0.875rem',
                          textAlign: 'center',
                        }}
                      >
                        Upload
                      </Link>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
