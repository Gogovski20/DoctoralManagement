import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { studentService } from '../api/studentService';
import { useAuth } from '../context/AuthContext';

export default function CompleteDoctoralProjectPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [finalReportNotes, setFinalReportNotes] = useState('');

  useEffect(() => {
    fetchProject();
  }, [id]);

  const fetchProject = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getDoctoralProjectById(id);
      setProject(data);
    } catch (err) {
      console.error('Failed to fetch project:', err);
      setError(err.response?.data?.message || 'Failed to load project details');
    } finally {
      setLoading(false);
    }
  };

  const handleCompleteProject = async (e) => {
    e.preventDefault();

    if (!window.confirm('Are you sure you want to complete this doctoral project? This action cannot be undone.')) {
      return;
    }

    setSubmitting(true);
    setError('');

    try {
      const result = await studentService.completeDoctoralProject(id, finalReportNotes.trim());

      if (result.projectId) {
        alert('Project completed successfully!');
        // Navigate based on user role
        const backPath = user?.role === 'Mentor' ? '/mentor/dashboard' : '/admin/doctoral-projects';
        navigate(backPath);
      } else {
        setError(result.message || 'Failed to complete project');
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.message || 'Failed to complete project';
      setError(errorMsg);
      console.error('Error completing project:', err);
    } finally {
      setSubmitting(false);
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
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch (e) {
      return 'Invalid Date';
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '900px', margin: '0 auto', textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
          <p>Loading project details...</p>
        </div>
      </div>
    );
  }

  if (error && !project) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '900px', margin: '0 auto' }}>
          <Link to={user?.role === 'Mentor' ? '/mentor/dashboard' : '/admin/doctoral-projects'} style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back
          </Link>
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1.5rem',
            borderRadius: '0.5rem',
            marginTop: '1rem',
          }}>
            <h3 style={{ margin: '0 0 0.5rem 0' }}>Error</h3>
            <p style={{ margin: 0 }}>{error}</p>
          </div>
        </div>
      </div>
    );
  }

  if (!project) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '900px', margin: '0 auto' }}>
          <Link to={user?.role === 'Mentor' ? '/mentor/dashboard' : '/admin/doctoral-projects'} style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back
          </Link>
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1.5rem',
            borderRadius: '0.5rem',
            marginTop: '1rem',
          }}>
            Project not found
          </div>
        </div>
      </div>
    );
  }

  const statusColor = getStatusColor(project.status);
  const backPath = user?.role === 'Mentor' ? '/mentor/dashboard' : '/admin/doctoral-projects';

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to={backPath} style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back
          </Link>

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginTop: '1rem' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 0.5rem 0' }}>
                Complete Project: {project.title}
              </h1>
              <p style={{ color: '#6b7280', fontSize: '1rem', margin: 0 }}>
                Student: <strong>{project.studentName}</strong>
              </p>
            </div>
            <span style={{
              display: 'inline-block',
              padding: '0.5rem 1rem',
              borderRadius: '9999px',
              backgroundColor: statusColor + '20',
              color: statusColor,
              fontWeight: '600',
              fontSize: '0.875rem',
              whiteSpace: 'nowrap',
            }}>
              {getStatusLabel(project.status)}
            </span>
          </div>
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

        {/* Main Content */}
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem' }}>
          {/* Left Column - Project Info & Completion Form */}
          <div>
            {/* Project Information */}
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              marginBottom: '1.5rem',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1rem 0' }}>
                Project Information
              </h2>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
                <div>
                  <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                    Research Area
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {project.researchArea || 'N/A'}
                  </p>
                </div>

                <div>
                  <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                    Status
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {getStatusLabel(project.status)}
                  </p>
                </div>

                <div>
                  <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                    Created
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {formatDate(project.createdAt)}
                  </p>
                </div>

                {project.submittedAt && (
                  <div>
                    <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                      Submitted
                    </p>
                    <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                      {formatDate(project.submittedAt)}
                    </p>
                  </div>
                )}
              </div>
            </div>

            {/* Documents */}
            {project.documents && project.documents.length > 0 && (
              <div style={{
                backgroundColor: 'white',
                borderRadius: '0.75rem',
                padding: '1.5rem',
                border: '1px solid #e5e7eb',
                marginBottom: '1.5rem',
                boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
              }}>
                <h2 style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1rem 0' }}>
                  Documents ({project.documents.length})
                </h2>

                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  {project.documents.map((doc) => (
                    <div
                      key={doc.id}
                      style={{
                        border: '1px solid #e5e7eb',
                        borderRadius: '0.5rem',
                        padding: '1rem',
                        backgroundColor: '#f9fafb',
                      }}
                    >
                      <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                        <span style={{ fontSize: '1.25rem' }}>📄</span>
                        <div>
                          <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.25rem 0' }}>
                            {doc.fileName}
                          </h4>
                          <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                            Uploaded: {formatDate(doc.uploadedAt)}
                          </p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Completion Form */}
            <form onSubmit={handleCompleteProject} style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1.5rem 0' }}>
                Complete Project
              </h2>

              {/* Final Report Notes */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{
                  display: 'block',
                  fontSize: '0.875rem',
                  fontWeight: '500',
                  color: '#374151',
                  marginBottom: '0.5rem',
                }}>
                  Final Report Notes (Optional, Max 2000 characters)
                </label>
                <textarea
                  value={finalReportNotes}
                  onChange={(e) => setFinalReportNotes(e.target.value)}
                  placeholder="Add any final notes, comments, or observations about the completed project..."
                  maxLength={2000}
                  style={{
                    width: '100%',
                    padding: '0.5rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    minHeight: '120px',
                    boxSizing: 'border-box',
                    fontFamily: 'inherit',
                  }}
                />
                <p style={{ fontSize: '0.75rem', color: '#6b7280', margin: '0.25rem 0 0 0' }}>
                  {finalReportNotes.length}/2000 characters
                </p>
              </div>

              {/* Warning Alert */}
              <div style={{
                backgroundColor: '#fffbeb',
                border: '1px solid #fcd34d',
                borderRadius: '0.5rem',
                padding: '1rem',
                marginBottom: '1.5rem',
              }}>
                <p style={{ color: '#92400e', fontSize: '0.875rem', margin: 0 }}>
                  ⚠️ <strong>Warning:</strong> Completing a doctoral project is a final action that cannot be undone. The student will receive the remaining ECTS credits (27 ECTS) upon completion.
                </p>
              </div>

              {/* Buttons */}
              <div style={{ display: 'flex', gap: '1rem' }}>
                <button
                  type="button"
                  onClick={() => navigate(backPath)}
                  style={{
                    flex: 1,
                    background: '#e5e7eb',
                    color: '#1f2937',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: 'pointer',
                    fontWeight: '600',
                  }}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{
                    flex: 1,
                    background: 'linear-gradient(90deg, #10b981 0%, #059669 100%)',
                    color: 'white',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: submitting ? 'not-allowed' : 'pointer',
                    fontWeight: '600',
                    opacity: submitting ? 0.5 : 1,
                  }}
                >
                  {submitting ? 'Completing...' : 'Complete Project'}
                </button>
              </div>
            </form>
          </div>

          {/* Right Column - Info */}
          <div>
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h3 style={{ fontSize: '1rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1.5rem 0' }}>
                What Happens Next?
              </h3>

              <div style={{ fontSize: '0.875rem', color: '#6b7280', lineHeight: '1.6' }}>
                <div style={{
                  backgroundColor: '#f0fdf4',
                  border: '1px solid #dcfce7',
                  borderRadius: '0.5rem',
                  padding: '1rem',
                  marginBottom: '1rem',
                }}>
                  <h4 style={{ fontWeight: '600', color: '#166534', margin: '0 0 0.5rem 0' }}>✓ Upon Completion</h4>
                  <ul style={{ margin: 0, paddingLeft: '1.5rem' }}>
                    <li>Project status changes to "Completed"</li>
                    <li>Student receives 27 ECTS credits</li>
                    <li>Total doctoral project ECTS: 41</li>
                  </ul>
                </div>

                <div style={{
                  backgroundColor: '#fef2f2',
                  border: '1px solid #fecaca',
                  borderRadius: '0.5rem',
                  padding: '1rem',
                }}>
                  <h4 style={{ fontWeight: '600', color: '#b91c1c', margin: '0 0 0.5rem 0' }}>⚠ Important</h4>
                  <p style={{ margin: 0 }}>This action is permanent. Ensure the project meets all requirements before completing.</p>
                </div>
              </div>

              {/* Status Badge */}
              <div style={{
                backgroundColor: statusColor + '10',
                border: `1px solid ${statusColor}40`,
                borderRadius: '0.5rem',
                padding: '1rem',
                marginTop: '1.5rem',
              }}>
                <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0 0 0.5rem 0' }}>
                  Current Status
                </p>
                <p style={{ color: statusColor, fontWeight: '600', margin: 0 }}>
                  {getStatusLabel(project.status)}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}