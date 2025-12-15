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
      const data = await studentService.getMyDoctoralProjects();
      setProjects(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load doctoral projects');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    if (!status) return '#6b7280';
    const s = status.toString().toLowerCase();
    switch (s) {
      case 'draft': return '#6b7280';
      case 'submitted': return '#3b82f6';
      case 'underreview':
      case 'under_review': return '#f59e0b';
      case 'approved': return '#10b981';
      case 'completed': return '#16a34a';
      case 'defensechangesrequired':
      case 'defense_changes_required': return '#f97316';
      case 'rejected': return '#ef4444';
      default: return '#6b7280';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return 'Draft';
    return status
      .toString()
      .replace(/([A-Z])/g, ' $1')
      .replace(/_/g, ' ')
      .trim()
      .replace(/\b\w/g, l => l.toUpperCase());
  };

  const canUploadThesis = (status) => {
    if (!status) return false;
    const s = status.toString().toLowerCase();
    return s === 'completed' || s === 'defensechangesrequired' || s === 'defense_changes_required';
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      return new Date(dateString).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch {
      return 'Invalid Date';
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>

        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/dashboard" style={{ color: '#0d9488' }}>
            ← Back to Dashboard
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', marginTop: '0.5rem' }}>
            Doctoral Projects
          </h1>
          <p style={{ color: '#6b7280' }}>
            Manage your doctoral research projects
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

        {/* Projects */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          border: '1px solid #e5e7eb',
          padding: '1.5rem'
        }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            marginBottom: '1.5rem'
          }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold' }}>
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
            <div style={{ textAlign: 'center', padding: '3rem', color: '#6b7280' }}>
              ⏳ Loading projects...
            </div>
          ) : projects.length === 0 ? (
            <div style={{ textAlign: 'center', padding: '3rem', color: '#6b7280' }}>
              <p>No doctoral projects yet.</p>
              <Link
                to="/doctoral-project/new"
                style={{
                  backgroundColor: '#0d9488',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  textDecoration: 'none',
                }}
              >
                Create your first project
              </Link>
            </div>
          ) : (
            <div style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))',
              gap: '1.5rem'
            }}>
              {projects.map(project => (
                <div
                  key={project.id}
                  style={{
                    border: '1px solid #e5e7eb',
                    borderRadius: '0.75rem',
                    padding: '1.5rem',
                    transition: '0.2s'
                  }}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <div>
                      <h3 style={{ fontWeight: '600' }}>{project.title}</h3>
                      <p style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                        {project.researchArea}
                      </p>
                    </div>

                    <span style={{
                      padding: '0.25rem 0.75rem',
                      borderRadius: '9999px',
                      backgroundColor: getStatusColor(project.status) + '20',
                      color: getStatusColor(project.status),
                      fontSize: '0.75rem',
                      fontWeight: '500'
                    }}>
                      {getStatusLabel(project.status)}
                    </span>
                  </div>

                  <div style={{ marginTop: '0.75rem' }}>
                    <p style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                      Mentor: <strong>{project.mentorName || 'N/A'}</strong>
                    </p>
                    <p style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                      Created: <strong>{formatDate(project.createdAt)}</strong>
                    </p>
                  </div>

                  <div style={{ display: 'flex', gap: '0.5rem', marginTop: '1rem' }}>
                    <Link
                      to={`/doctoral-project/${project.id}`}
                      style={{
                        flex: 1,
                        backgroundColor: '#0d9488',
                        color: 'white',
                        padding: '0.5rem',
                        borderRadius: '0.5rem',
                        textAlign: 'center',
                        textDecoration: 'none',
                        fontSize: '0.875rem',
                      }}
                    >
                      View
                    </Link>

                    {canUploadThesis(project.status) && (
                      <Link
                        to={`/doctoral-project/${project.id}/upload-thesis`}
                        style={{
                          flex: 1,
                          backgroundColor: '#7c3aed',
                          color: 'white',
                          padding: '0.5rem',
                          borderRadius: '0.5rem',
                          textAlign: 'center',
                          textDecoration: 'none',
                          fontSize: '0.875rem',
                        }}
                      >
                        Upload Thesis
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
