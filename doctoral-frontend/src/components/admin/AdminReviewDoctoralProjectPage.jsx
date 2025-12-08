import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminReviewDoctoralProjectPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  // Form state
  const [newStatus, setNewStatus] = useState('UnderReview');
  const [committeeNotes, setCommitteeNotes] = useState('');
  const [documentStatus, setDocumentStatus] = useState('');
  const [reviewComment, setReviewComment] = useState('');

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

  const handleSubmitReview = async (e) => {
    e.preventDefault();

    if (!newStatus) {
      setError('Please select a new status');
      return;
    }

    setSubmitting(true);
    setError('');

    try {
      const result = await studentService.reviewDoctoralProject(id, {
        newStatus,
        committeeNotes: committeeNotes.trim(),
        documentStatus: documentStatus || null,
        reviewComment: reviewComment.trim(),
      });

      if (result.id) {
        alert('Project reviewed successfully!');
        navigate('/admin/doctoral-projects');
      } else {
        setError(result.message || 'Failed to review project');
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.message || 'Failed to review project';
      setError(errorMsg);
      console.error('Error reviewing project:', err);
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
          <Link to="/admin/doctoral-projects" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Projects
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
          <Link to="/admin/doctoral-projects" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Projects
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

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin/doctoral-projects" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Projects
          </Link>

          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginTop: '1rem' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 0.5rem 0' }}>
                Review: {project.title}
              </h1>
              <p style={{ color: '#6b7280', fontSize: '1rem', margin: 0 }}>
                {project.researchArea}
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
          {/* Left Column - Project Info & Review Form */}
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
                    Student
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {project.studentName || 'N/A'}
                  </p>
                </div>

                <div>
                  <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.25rem 0' }}>
                    Mentor
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {project.mentorName || 'N/A'}
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
                      <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '0.5rem' }}>
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

            {/* Review Form */}
            <form onSubmit={handleSubmitReview} style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1.5rem 0' }}>
                Review & Decision
              </h2>

              {/* New Status */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{
                  display: 'block',
                  fontSize: '0.875rem',
                  fontWeight: '500',
                  color: '#374151',
                  marginBottom: '0.5rem',
                }}>
                  Project Status *
                </label>
                <select
                  value={newStatus}
                  onChange={(e) => setNewStatus(e.target.value)}
                  style={{
                    width: '100%',
                    padding: '0.5rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    boxSizing: 'border-box',
                  }}
                  required
                >
                  <option value="UnderReview">Under Review</option>
                  <option value="Approved">Approved</option>
                  <option value="ChangesRequested">Changes Requested</option>
                  <option value="Rejected">Rejected</option>
                </select>
              </div>

              {/* Document Status (optional) */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{
                  display: 'block',
                  fontSize: '0.875rem',
                  fontWeight: '500',
                  color: '#374151',
                  marginBottom: '0.5rem',
                }}>
                  Document Status (Optional)
                </label>
                <select
                  value={documentStatus}
                  onChange={(e) => setDocumentStatus(e.target.value)}
                  style={{
                    width: '100%',
                    padding: '0.5rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    boxSizing: 'border-box',
                  }}
                >
                  <option value="">No Change</option>
                  <option value="Approved">Approved</option>
                  <option value="Rejected">Rejected</option>
                  <option value="Pending">Pending Review</option>
                </select>
              </div>

              {/* Review Comment */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{
                  display: 'block',
                  fontSize: '0.875rem',
                  fontWeight: '500',
                  color: '#374151',
                  marginBottom: '0.5rem',
                }}>
                  Document Review Comment (Max 2000 characters)
                </label>
                <textarea
                  value={reviewComment}
                  onChange={(e) => setReviewComment(e.target.value)}
                  placeholder="Provide detailed feedback on the proposal document..."
                  maxLength={2000}
                  style={{
                    width: '100%',
                    padding: '0.5rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    minHeight: '100px',
                    boxSizing: 'border-box',
                    fontFamily: 'inherit',
                  }}
                />
                <p style={{ fontSize: '0.75rem', color: '#6b7280', margin: '0.25rem 0 0 0' }}>
                  {reviewComment.length}/2000 characters
                </p>
              </div>

              {/* Committee Notes */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{
                  display: 'block',
                  fontSize: '0.875rem',
                  fontWeight: '500',
                  color: '#374151',
                  marginBottom: '0.5rem',
                }}>
                  Committee Notes (Max 2000 characters)
                </label>
                <textarea
                  value={committeeNotes}
                  onChange={(e) => setCommitteeNotes(e.target.value)}
                  placeholder="Add any additional comments or notes from the committee..."
                  maxLength={2000}
                  style={{
                    width: '100%',
                    padding: '0.5rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    minHeight: '100px',
                    boxSizing: 'border-box',
                    fontFamily: 'inherit',
                  }}
                />
                <p style={{ fontSize: '0.75rem', color: '#6b7280', margin: '0.25rem 0 0 0' }}>
                  {committeeNotes.length}/2000 characters
                </p>
              </div>

              {/* Buttons */}
              <div style={{ display: 'flex', gap: '1rem' }}>
                <button
                  type="button"
                  onClick={() => navigate('/admin/doctoral-projects')}
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
                    background: 'linear-gradient(90deg, #f59e0b 0%, #d97706 100%)',
                    color: 'white',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: submitting ? 'not-allowed' : 'pointer',
                    fontWeight: '600',
                    opacity: submitting ? 0.5 : 1,
                  }}
                >
                  {submitting ? 'Submitting...' : 'Submit Review'}
                </button>
              </div>
            </form>
          </div>

          {/* Right Column - Status Info */}
          <div>
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h3 style={{ fontSize: '1rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1.5rem 0' }}>
                Review Guide
              </h3>

              <div style={{ fontSize: '0.875rem', color: '#6b7280', lineHeight: '1.6' }}>
                <div style={{ marginBottom: '1.5rem' }}>
                  <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.5rem 0' }}>Under Review</h4>
                  <p style={{ margin: 0 }}>Project is being evaluated. No final decision yet.</p>
                </div>

                <div style={{ marginBottom: '1.5rem' }}>
                  <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.5rem 0' }}>Approved</h4>
                  <p style={{ margin: 0 }}>Project is approved. Student receives 14 ECTS and can proceed to completion phase.</p>
                </div>

                <div style={{ marginBottom: '1.5rem' }}>
                  <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.5rem 0' }}>Changes Requested</h4>
                  <p style={{ margin: 0 }}>Student must revise and resubmit the project with requested changes.</p>
                </div>

                <div>
                  <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.5rem 0' }}>Rejected</h4>
                  <p style={{ margin: 0 }}>Project does not meet requirements and cannot proceed.</p>
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