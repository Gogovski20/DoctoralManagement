import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function ApplicationDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [application, setApplication] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [documents, setDocuments] = useState([]);

  useEffect(() => {
    fetchApplication();
  }, [id]);

  const fetchApplication = async () => {
    try {
      setLoading(true);
      const data = await studentService.getApplicationById(id);
      setApplication(data);
      setDocuments(data.documents || []);
    } catch (err) {
      setError('Failed to load application details');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    switch(status?.toLowerCase()) {
      case 'draft': return '#f59e0b';
      case 'submitted': return '#3b82f6';
      case 'under_review': return '#8b5cf6';
      case 'approved': return '#10b981';
      case 'final_accepted': return '#059669';
      case 'rejected': return '#ef4444';
      default: return '#6b7280';
    }
  };

  const handleSubmitApplication = async () => {
    if (!window.confirm('Are you sure you want to submit this application? You cannot make changes after submission.')) {
      return;
    }

    try {
      setLoading(true);
      const result = await studentService.submitApplication(id);
      if (result.success || result.id) {
        alert('Application submitted successfully!');
        fetchApplication();
      }
    } catch (err) {
      setError('Failed to submit application');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        Loading application details...
      </div>
    );
  }

  if (!application) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Application not found</p>
        <Link to="/dashboard" style={{ color: '#0d9488' }}>
          Back to Dashboard
        </Link>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
                Application Details
              </h1>
              <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
                Application ID: {application.id}
              </p>
            </div>
            <span style={{
              padding: '0.5rem 1rem',
              borderRadius: '9999px',
              backgroundColor: getStatusColor(application.applicationStatus) + '20',
              color: getStatusColor(application.applicationStatus),
              fontWeight: '600',
              fontSize: '0.875rem',
            }}>
              {application.applicationStatus?.replace(/_/g, ' ') || application.applicationStatus === 'Draft'}
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

        {/* Application Information */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
            Program Information
          </h2>
          
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '1rem' }}>
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Program</p>
              <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                {application.programName || 'N/A'}
              </p>
            </div>
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Scientific Area</p>
              <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                {application.scientificArea || 'N/A'}
              </p>
            </div>
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Preferred Mentor</p>
              <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                {application.preferredMentorName || 'Not specified'}
              </p>
            </div>
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Created Date</p>
              <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                {new Date(application.applicationDate).toLocaleDateString()}
              </p>
            </div>
          </div>
        </div>

        {/* Documents Section */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
            Documents
          </h2>

          {documents.length === 0 ? (
            <p style={{ color: '#6b7280', textAlign: 'center', padding: '2rem' }}>
              No documents uploaded yet
            </p>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
              {documents.map((doc) => (
                <div key={doc.id} style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  padding: '1rem',
                  border: '1px solid #e5e7eb',
                  borderRadius: '0.5rem',
                }}>
                  <div>
                    <p style={{ fontWeight: '500', color: '#1f2937', margin: 0 }}>
                      {doc.documentType?.replace(/([A-Z])/g, ' $1').trim()}
                    </p>
                    <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0.25rem 0 0 0' }}>
                      Uploaded: {new Date(doc.uploadedAt).toLocaleDateString()}
                    </p>
                  </div>
                  <button
                    onClick={() => window.open(doc.filePath, '_blank')}
                    style={{
                      backgroundColor: '#0d9488',
                      color: 'white',
                      padding: '0.5rem 1rem',
                      borderRadius: '0.5rem',
                      border: 'none',
                      cursor: 'pointer',
                      fontSize: '0.875rem',
                    }}
                  >
                    View Document
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Actions */}
        {application.status === 'Draft' && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h3 style={{ fontSize: '1.125rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
              Application Actions
            </h3>
            <div style={{ display: 'flex', gap: '1rem' }}>
              <Link
                to={`/applications/${id}/upload`}
                style={{
                  backgroundColor: '#3b82f6',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  textDecoration: 'none',
                  fontWeight: '500',
                }}
              >
                Upload Documents
              </Link>
              <button
                onClick={handleSubmitApplication}
                disabled={loading || documents.length < 3} // Assuming 3 required documents
                style={{
                  backgroundColor: '#10b981',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: documents.length >= 3 ? 'pointer' : 'not-allowed',
                  fontWeight: '500',
                  opacity: documents.length >= 3 ? 1 : 0.5,
                }}
              >
                {loading ? 'Submitting...' : 'Submit Application'}
              </button>
            </div>
            {documents.length < 3 && (
              <p style={{ color: '#ef4444', fontSize: '0.875rem', marginTop: '1rem' }}>
                You need to upload all required documents before submitting.
              </p>
            )}
          </div>
        )}
      </div>
    </div>
  );
}