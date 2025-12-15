import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function DoctoralProjectDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [downloadingFile, setDownloadingFile] = useState(null);

  useEffect(() => {
    fetchProject();
  }, [id]);

  const fetchProject = async () => {
    try {
      setLoading(true);
      setError('');
      console.log('Fetching project:', id);
      const data = await studentService.getDoctoralProjectById(id);
      console.log('Project data:', data);
      setProject(data);
    } catch (err) {
      console.error('Failed to fetch project:', err);
      setError(err.response?.data?.message || 'Failed to load project details');
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

  const getDocumentTypeLabel = (type) => {
    const typeStr = type?.toString?.() || type || '';
    return typeStr
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

  const handleDownloadDocument = async (doc) => {
    try {
      setDownloadingFile(doc.id);
      const response = await fetch(`/api/DoctoralProjects/${project.id}/documents/${doc.id}/download`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      
      if (!response.ok) throw new Error('Download failed');
      
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = doc.fileName;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (err) {
      console.error('Download error:', err);
      alert('Failed to download document');
    } finally {
      setDownloadingFile(null);
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

  if (error) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '900px', margin: '0 auto' }}>
          <Link to="/doctoral-project" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
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
          <Link to="/doctoral-project" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
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
          <Link to="/doctoral-project" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Projects
          </Link>
          
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginTop: '1rem' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 0.5rem 0' }}>
                {project.title}
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

        {/* Main Content */}
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem' }}>
          {/* Left Column - Project Details */}
          <div>
            {/* Project Information Card */}
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
                    Mentor
                  </p>
                  <p style={{ fontWeight: '600', color: '#1f2937', margin: 0 }}>
                    {project.mentorName || 'N/A'}
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

            {/* Documents Section */}
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
                <h2 style={{ fontSize: '1.25rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
                  Documents
                </h2>
                <span style={{
                  backgroundColor: '#e0f2fe',
                  color: '#0369a1',
                  padding: '0.25rem 0.75rem',
                  borderRadius: '9999px',
                  fontSize: '0.75rem',
                  fontWeight: '600',
                }}>
                  {project.documents?.length || 0}
                </span>
              </div>

              {project.documents && project.documents.length > 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  {project.documents.map((doc) => (
                    <div
                      key={doc.id}
                      style={{
                        border: '1px solid #e5e7eb',
                        borderRadius: '0.5rem',
                        padding: '1rem',
                        backgroundColor: '#f9fafb',
                        transition: 'all 0.2s',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#f3f4f6';
                        e.currentTarget.style.borderColor = '#0d9488';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = '#f9fafb';
                        e.currentTarget.style.borderColor = '#e5e7eb';
                      }}
                    >
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                        <div style={{ flex: 1 }}>
                          <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '0.5rem' }}>
                            <span style={{ fontSize: '1.25rem' }}>📄</span>
                            <div>
                              <h4 style={{ fontWeight: '600', color: '#1f2937', margin: '0 0 0.25rem 0' }}>
                                {doc.fileName}
                              </h4>
                              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                                {getDocumentTypeLabel(doc.type)}
                              </p>
                            </div>
                          </div>
                          <div style={{ display: 'flex', gap: '1rem', fontSize: '0.75rem', color: '#6b7280', marginTop: '0.75rem' }}>
                            <span>Uploaded: {formatDate(doc.uploadedAt)}</span>
                            {doc.contentType && <span>{doc.contentType}</span>}
                          </div>
                        </div>
                        
                        <button
                          onClick={() => handleDownloadDocument(doc)}
                          disabled={downloadingFile === doc.id}
                          style={{
                            backgroundColor: '#0d9488',
                            color: 'white',
                            padding: '0.5rem 1rem',
                            borderRadius: '0.5rem',
                            border: 'none',
                            cursor: downloadingFile === doc.id ? 'not-allowed' : 'pointer',
                            fontWeight: '500',
                            fontSize: '0.875rem',
                            whiteSpace: 'nowrap',
                            marginLeft: '1rem',
                            opacity: downloadingFile === doc.id ? 0.5 : 1,
                          }}
                        >
                          {downloadingFile === doc.id ? 'Downloading...' : 'Download'}
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div style={{
                  textAlign: 'center',
                  color: '#6b7280',
                  padding: '2rem',
                  backgroundColor: '#f9fafb',
                  borderRadius: '0.5rem',
                }}>
                  <p style={{ margin: 0 }}>No documents uploaded yet</p>
                </div>
              )}
            </div>
          </div>

          {/* Right Column - Actions */}
          <div>
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            }}>
              <h3 style={{ fontSize: '1rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 1.5rem 0' }}>
                Actions
              </h3>
              
              <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                {/* Edit/Upload buttons only if Draft or ChangesRequested */}
                {(project.status === 'Draft' || project.status === 'ChangesRequested') && (
                  <>
                    <Link
                      to={`/doctoral-project/${project.id}/upload`}
                      style={{
                        backgroundColor: '#3b82f6',
                        color: 'white',
                        padding: '0.75rem',
                        borderRadius: '0.5rem',
                        textDecoration: 'none',
                        fontWeight: '500',
                        fontSize: '0.875rem',
                        textAlign: 'center',
                      }}
                    >
                      📤 Upload Document
                    </Link>
                    
                    <button
                      style={{
                        backgroundColor: '#ef4444',
                        color: 'white',
                        padding: '0.75rem',
                        borderRadius: '0.5rem',
                        border: 'none',
                        fontWeight: '500',
                        fontSize: '0.875rem',
                        cursor: 'pointer',
                      }}
                      onClick={() => {
                        if (window.confirm('Are you sure you want to delete this project? This action cannot be undone.')) {
                          console.log('Delete project:', project.id);
                        }
                      }}
                    >
                      🗑️ Delete Project
                    </button>
                  </>
                )}

                {/* View only button if Submitted or higher status */}
                {(project.status === 'Submitted' || project.status === 'UnderReview' || project.status === 'Approved') && (
                  <div style={{
                    backgroundColor: '#f0fdf4',
                    border: '1px solid #dcfce7',
                    color: '#166534',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    fontSize: '0.875rem',
                    textAlign: 'center',
                  }}>
                    ✓ Project submitted
                  </div>
                )}

                {project.status === 'Rejected' && (
                  <div style={{
                    backgroundColor: '#fef2f2',
                    border: '1px solid #fecaca',
                    color: '#b91c1c',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    fontSize: '0.875rem',
                    textAlign: 'center',
                  }}>
                    ✗ Project rejected
                  </div>
                )}
              </div>

              {/* Status Badge Info */}
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