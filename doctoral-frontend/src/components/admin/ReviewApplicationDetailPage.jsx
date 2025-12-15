import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';


export default function ReviewApplicationDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [application, setApplication] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [reviewing, setReviewing] = useState(false);
  const [downloadingDocId, setDownloadingDocId] = useState(null);
  const [formData, setFormData] = useState({
    newStatus: '',
    reviewComments: '',
    hasRequiredPublications: false,
  });


  useEffect(() => {
    fetchApplication();
  }, [id]);


  const fetchApplication = async () => {
    try {
      setLoading(true);
      const data = await studentService.getApplicationById(id);
      setApplication(data);
      
      // Initialize form with current data
      setFormData({
        newStatus: '',
        reviewComments: data.reviewComments || '',
        hasRequiredPublications: data.hasRequiredPublications || false,
      });
    } catch (err) {
      console.error('Failed to fetch application:', err);
      setError('Failed to load application details');
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
      case 'preliminaryaccepted':
      case 'preliminary_accepted':
        return '#8b5cf6';
      case 'finalaccepted':
      case 'final_accepted':
        return '#059669';
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


  const isReviewable = application && 
    ['Submitted', 'UnderReview', 'PreliminaryAccepted'].includes(application.applicationStatus);


  const getValidNextStatuses = () => {
    if (!application) return [];
    
    const transitions = {
      'Submitted': ['UnderReview', 'Rejected'],
      'UnderReview': ['PreliminaryAccepted', 'Rejected'],
      'PreliminaryAccepted': ['FinalAccepted', 'Rejected'],
    };
    
    return transitions[application.applicationStatus] || [];
  };


  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };


  const handleDownloadDocument = async (doc) => {
    try {
      setDownloadingDocId(doc.id);
      await studentService.downloadApplicationDocument(id, doc.id, doc.fileName);
    } catch (err) {
      console.error('Failed to download document:', err);
      setError(`Failed to download ${doc.fileName}`);
    } finally {
      setDownloadingDocId(null);
    }
  };


  const handleSubmit = async (e) => {
    e.preventDefault();


    if (!formData.newStatus) {
      setError('Please select a new status');
      return;
    }


    if (!formData.reviewComments.trim()) {
      setError('Review comments are required');
      return;
    }


    try {
      setReviewing(true);
      setError('');
      
      const reviewData = {
        id: parseInt(id),
        newStatus: formData.newStatus,
        reviewComments: formData.reviewComments,
        hasRequiredPublications: formData.hasRequiredPublications,
      };


      const result = await studentService.reviewApplication(id, reviewData);
      alert('Application reviewed successfully');
      navigate('/admin/applications');
    } catch (err) {
      console.error('Failed to review application:', err);
      setError(err.response?.data?.message || 'Failed to review application');
    } finally {
      setReviewing(false);
    }
  };


  if (loading) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '1000px', margin: '0 auto', textAlign: 'center', paddingTop: '3rem' }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
          <p style={{ color: '#6b7280' }}>Loading application details...</p>
        </div>
      </div>
    );
  }


  if (!application) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '1000px', margin: '0 auto', textAlign: 'center', paddingTop: '3rem' }}>
          <p style={{ color: '#6b7280', marginBottom: '1rem' }}>Application not found</p>
          <Link to="/admin/applications" style={{ color: '#0d9488', fontWeight: '500' }}>
            ← Back to Applications
          </Link>
        </div>
      </div>
    );
  }


  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1000px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin/applications" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block', fontWeight: '500' }}>
            ← Back to Applications
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Review Application
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Application ID: {application.id}
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


        {!isReviewable && (
          <div style={{
            backgroundColor: '#fef3c7',
            border: '1px solid #fcd34d',
            color: '#92400e',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
          }}>
            ⚠️ This application cannot be reviewed. Only applications in Submitted, Under Review, or Preliminary Accepted status can be reviewed.
          </div>
        )}


        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '1.5rem' }}>
          {/* Application Details */}
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
              Student Information
            </h2>


            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Name</p>
                <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                  {application.studentName}
                </p>
              </div>


              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Email</p>
                <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                  {application.studentEmail}
                </p>
              </div>


              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Meets Grade Requirements</p>
                <p style={{
                  fontWeight: '500',
                  color: application.meetsGradeRequirements ? '#166534' : '#991b1b',
                  margin: '0.25rem 0 0 0'
                }}>
                  {application.meetsGradeRequirements ? '✓ Yes' : '✗ No'}
                </p>
              </div>
            </div>
          </div>


          {/* Program & Status */}
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
              Program & Status
            </h2>


            <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Program</p>
                <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                  {application.programName}
                </p>
                <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0.25rem 0 0 0' }}>
                  {application.scientificArea}
                </p>
              </div>


              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Current Status</p>
                <span style={{
                  display: 'inline-block',
                  padding: '0.25rem 0.75rem',
                  borderRadius: '9999px',
                  backgroundColor: getStatusColor(application.applicationStatus) + '20',
                  color: getStatusColor(application.applicationStatus),
                  fontWeight: '500',
                  fontSize: '0.75rem',
                  marginTop: '0.25rem',
                }}>
                  {getStatusLabel(application.applicationStatus)}
                </span>
              </div>


              <div>
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>Applied Date</p>
                <p style={{ fontWeight: '500', color: '#1f2937', margin: '0.25rem 0 0 0' }}>
                  {formatDate(application.applicationDate)}
                </p>
              </div>
            </div>
          </div>
        </div>


        {/* Documents Section */}
        {application.documents && application.documents.length > 0 && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
            marginBottom: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
              Submitted Documents
            </h2>


            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
              {application.documents.map((doc) => (
                <div key={doc.id} style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  padding: '1rem',
                  border: '1px solid #e5e7eb',
                  borderRadius: '0.5rem',
                  backgroundColor: '#fafafa',
                }}>
                  <div style={{ flex: 1 }}>
                    <p style={{ fontWeight: '500', color: '#1f2937', margin: 0 }}>
                      {doc.documentType?.replace(/([A-Z])/g, ' $1').trim()}
                    </p>
                    <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0.25rem 0 0 0' }}>
                      📄 {doc.fileName}
                    </p>
                  </div>
                  <button
                    onClick={() => handleDownloadDocument(doc)}
                    disabled={downloadingDocId === doc.id}
                    style={{
                      backgroundColor: downloadingDocId === doc.id ? '#9ca3af' : '#0d9488',
                      color: 'white',
                      padding: '0.5rem 1rem',
                      borderRadius: '0.5rem',
                      border: 'none',
                      textDecoration: 'none',
                      fontWeight: '500',
                      fontSize: '0.875rem',
                      cursor: downloadingDocId === doc.id ? 'not-allowed' : 'pointer',
                      whiteSpace: 'nowrap',
                      marginLeft: '1rem',
                    }}
                  >
                    {downloadingDocId === doc.id ? '⬇ Downloading...' : '⬇ Download'}
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}


        {/* Review Form */}
        {isReviewable && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
              Application Review
            </h2>


            <form onSubmit={handleSubmit}>
              {/* New Status */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '600', color: '#374151', marginBottom: '0.5rem' }}>
                  New Status *
                </label>
                <select
                  name="newStatus"
                  value={formData.newStatus}
                  onChange={handleChange}
                  required
                  style={{
                    width: '100%',
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '0.875rem',
                    backgroundColor: 'white',
                    boxSizing: 'border-box',
                  }}
                >
                  <option value="">Select a status...</option>
                  {getValidNextStatuses().map((status) => (
                    <option key={status} value={status}>
                      {getStatusLabel(status)}
                    </option>
                  ))}
                </select>
                <p style={{ fontSize: '0.75rem', color: '#6b7280', margin: '0.5rem 0 0 0' }}>
                  Select the new status for this application after review
                </p>
              </div>


              {/* Review Comments */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '600', color: '#374151', marginBottom: '0.5rem' }}>
                  Review Comments *
                </label>
                <textarea
                  name="reviewComments"
                  value={formData.reviewComments}
                  onChange={handleChange}
                  placeholder="Enter your review comments and feedback..."
                  rows="5"
                  required
                  style={{
                    width: '100%',
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '0.875rem',
                    fontFamily: 'inherit',
                    boxSizing: 'border-box',
                    resize: 'vertical',
                  }}
                />
                <p style={{ fontSize: '0.75rem', color: '#6b7280', margin: '0.5rem 0 0 0' }}>
                  Provide detailed feedback on the application, include reasons for acceptance or rejection
                </p>
              </div>


              {/* Has Required Publications */}
              <div style={{ marginBottom: '2rem', display: 'flex', alignItems: 'center', padding: '1rem', backgroundColor: '#f3f4f6', borderRadius: '0.5rem' }}>
                <input
                  type="checkbox"
                  id="hasPublications"
                  name="hasRequiredPublications"
                  checked={formData.hasRequiredPublications}
                  onChange={handleChange}
                  style={{
                    width: '1.25rem',
                    height: '1.25rem',
                    cursor: 'pointer',
                    marginRight: '0.75rem',
                  }}
                />
                <label htmlFor="hasPublications" style={{ fontSize: '0.875rem', fontWeight: '500', color: '#374151', cursor: 'pointer', margin: 0 }}>
                  ✓ Candidate has required publications
                </label>
              </div>


              {/* Submit Button */}
              <div style={{ display: 'flex', gap: '1rem', borderTop: '1px solid #e5e7eb', paddingTop: '1.5rem' }}>
                <button
                  type="submit"
                  disabled={reviewing}
                  style={{
                    backgroundColor: reviewing ? '#9ca3af' : '#0d9488',
                    color: 'white',
                    padding: '0.75rem 1.5rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: reviewing ? 'not-allowed' : 'pointer',
                    fontWeight: '600',
                    fontSize: '0.95rem',
                    transition: 'opacity 0.2s ease',
                    opacity: reviewing ? 0.6 : 1,
                  }}
                >
                  {reviewing ? '⏳ Submitting Review...' : '✓ Submit Review'}
                </button>
                <Link
                  to="/admin/applications"
                  style={{
                    backgroundColor: '#e5e7eb',
                    color: '#1f2937',
                    padding: '0.75rem 1.5rem',
                    borderRadius: '0.5rem',
                    textDecoration: 'none',
                    fontWeight: '600',
                    fontSize: '0.95rem',
                    display: 'inline-flex',
                    alignItems: 'center',
                    transition: 'background-color 0.2s ease',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = '#d1d5db'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = '#e5e7eb'}
                >
                  Cancel
                </Link>
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
}