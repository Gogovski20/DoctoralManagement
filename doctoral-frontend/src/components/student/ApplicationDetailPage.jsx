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

  const downloadDocument = async (doc) => {
    try {
      await studentService.downloadApplicationDocument(
        application.id,
        doc.id,
        doc.fileName || `${doc.documentType}.pdf`
      );
    } catch (err) {
      alert("Failed to download document.");
    }
  };

  if (loading) {
    return <div style={{ padding: '2rem', textAlign: 'center' }}>Loading application details...</div>;
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
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', margin: 0 }}>Application Details</h1>
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
              {application.applicationStatus?.replace(/_/g, ' ')}
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

        {/* Program Information */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h2 style={{ fontSize: '1.25rem', fontWeight: '600', marginBottom: '1rem' }}>
            Program Information
          </h2>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit,minmax(250px,1fr))', gap: '1rem' }}>
            <div><p>Program</p><strong>{application.programName}</strong></div>
            <div><p>Scientific Area</p><strong>{application.scientificArea}</strong></div>
            <div><p>Preferred Mentor</p><strong>{application.preferredMentorName}</strong></div>
            <div><p>Created Date</p><strong>{new Date(application.applicationDate).toLocaleDateString()}</strong></div>
          </div>
        </div>

        {/* Documents */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h2 style={{ fontSize: '1.25rem', fontWeight: '600', marginBottom: '1rem' }}>
            Documents
          </h2>

          {documents.length === 0 ? (
            <p style={{ textAlign: 'center', padding: '2rem' }}>No documents uploaded yet</p>
          ) : (
            documents.map(doc => (
              <div key={doc.id} style={{
                display: 'flex',
                justifyContent: 'space-between',
                padding: '1rem',
                border: '1px solid #e5e7eb',
                borderRadius: '0.5rem',
                marginBottom: '0.75rem',
              }}>
                <div>
                  <p style={{ fontWeight: 500 }}>{doc.documentType}</p>
                  <p style={{ color: '#6b7280' }}>
                    Uploaded: {new Date(doc.uploadedAt).toLocaleDateString()}
                  </p>
                </div>

                <button
                  onClick={() => downloadDocument(doc)}
                  style={{
                    backgroundColor: '#0d9488',
                    color: 'white',
                    padding: '0.5rem 1rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: 'pointer'
                  }}
                >
                  Download
                </button>
              </div>
            ))
          )}
        </div>

        {/* Actions */}
        {application.status === 'Draft' && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            padding: '1.5rem'
          }}>
            <h3 style={{ marginBottom: '1rem' }}>Application Actions</h3>

            <div style={{ display: 'flex', gap: '1rem' }}>
              <Link to={`/applications/${id}/upload`}>Upload Documents</Link>

              <button
                onClick={handleSubmitApplication}
                disabled={loading || documents.length < 3}
              >
                {loading ? 'Submitting...' : 'Submit Application'}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
