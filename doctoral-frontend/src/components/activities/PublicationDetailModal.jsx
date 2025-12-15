import React, { useState, useEffect } from 'react';
import { studentService } from '../../api/studentService';


export default function PublicationDetailModal({ publicationId, onClose }) {
  const [publication, setPublication] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [downloading, setDownloading] = useState(false);


  useEffect(() => {
    fetchPublication();
  }, [publicationId]);


  const fetchPublication = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getPublicationById(publicationId);
      setPublication(data);
    } catch (err) {
      setError(`Failed to load publication details: ${err.response?.data?.message || err.message}`);
    } finally {
      setLoading(false);
    }
  };


  const handleDownload = async () => {
    if (!publication?.document) {
      alert('No document available for download.');
      return;
    }

    try {
      setDownloading(true);
      await studentService.downloadPublicationDocument(
        publication.id,             
        publication.document.id,    
        publication.document.fileName 
      );
    } catch (err) {
      alert(`Download failed: ${err.message}`);
    } finally {
      setDownloading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return 'Invalid Date';
    }
  };


  const formatDateTime = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
    } catch {
      return 'Invalid Date';
    }
  };


  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1000,
        padding: '1rem',
      }}
      onClick={onClose}
    >
      <div
        style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 10px 25px rgba(0, 0, 0, 0.2)',
          padding: '2rem',
          maxWidth: '600px',
          width: '100%',
          maxHeight: '90vh',
          overflowY: 'auto',
        }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
          <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Publication Details
          </h2>
          <button
            onClick={onClose}
            style={{
              backgroundColor: 'transparent',
              border: 'none',
              fontSize: '1.5rem',
              cursor: 'pointer',
              color: '#6b7280',
            }}
          >
            ✕
          </button>
        </div>


        {error && (
          <div
            style={{
              backgroundColor: '#fef2f2',
              border: '1px solid #fecaca',
              color: '#b91c1c',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1rem',
            }}
          >
            {error}
          </div>
        )}


        {loading ? (
          <div style={{ textAlign: 'center', padding: '2rem', color: '#6b7280' }}>
            <div style={{ fontSize: '2rem', marginBottom: '1rem' }}>⏳</div>
            <p>Loading publication details...</p>
          </div>
        ) : publication ? (
          <div>
            {/* Title */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                Title
              </label>
              <p style={{ fontSize: '1.125rem', color: '#1f2937', margin: 0, fontWeight: '500' }}>
                {publication.title}
              </p>
            </div>


            {/* Student Name */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                Student
              </label>
              <p style={{ fontSize: '1rem', color: '#374151', margin: 0 }}>
                {publication.studentName}
              </p>
            </div>


            {/* Journal */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                Journal
              </label>
              <p style={{ fontSize: '1rem', color: '#374151', margin: 0 }}>
                {publication.journal}
              </p>
            </div>


            {/* Published Date */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                Published On
              </label>
              <p style={{ fontSize: '1rem', color: '#374151', margin: 0 }}>
                {formatDate(publication.publishedOn)}
              </p>
            </div>

            {/* Approval Status */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                Approval Status
              </label>
              <span
                style={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  gap: '0.5rem',
                  padding: '0.5rem 1rem',
                  borderRadius: '9999px',
                  backgroundColor: publication.isApproved ? '#10b98120' : '#ef444420',
                  color: publication.isApproved ? '#10b981' : '#ef4444',
                  fontSize: '0.875rem',
                  fontWeight: '600',
                  border: `1px solid ${publication.isApproved ? '#10b98140' : '#ef444440'}`,
                }}
              >
                {publication.isApproved ? (
                  <>
                    <span style={{ fontSize: '1.25rem' }}>✅</span>
                    Approved
                  </>
                ) : (
                  <>
                    <span style={{ fontSize: '1.25rem' }}>❌</span>
                    Pending Review
                  </>
                )}
              </span>
            </div>


            {/* Indexing Status */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.75rem' }}>
                Indexing Status
              </label>
              <div style={{ display: 'flex', gap: '1rem', flexDirection: 'column' }}>
                <span
                  style={{
                    display: 'inline-block',
                    padding: '0.25rem 0.75rem',
                    borderRadius: '9999px',
                    backgroundColor: publication.isIndexedInScopus ? '#10b98120' : '#6b728020',
                    color: publication.isIndexedInScopus ? '#10b981' : '#6b7280',
                    fontSize: '0.875rem',
                    fontWeight: '500',
                    width: 'fit-content',
                  }}
                >
                  {publication.isIndexedInScopus ? '✓ Scopus' : '✗ Not in Scopus'}
                </span>
                <span
                  style={{
                    display: 'inline-block',
                    padding: '0.25rem 0.75rem',
                    borderRadius: '9999px',
                    backgroundColor: publication.isIndexedInThomsonReuters ? '#10b98120' : '#6b728020',
                    color: publication.isIndexedInThomsonReuters ? '#10b981' : '#6b7280',
                    fontSize: '0.875rem',
                    fontWeight: '500',
                    width: 'fit-content',
                  }}
                >
                  {publication.isIndexedInThomsonReuters ? '✓ Thomson Reuters' : '✗ Not in Thomson Reuters'}
                </span>
              </div>
            </div>


            {/* Divider */}
            <div style={{ borderTop: '1px solid #e5e7eb', margin: '1.5rem 0' }} />


            {/* Document Section */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '1rem' }}>
                Publication Document
              </label>
              {publication.document ? (
                <div style={{
                  backgroundColor: '#f0fdf4',
                  border: '1px solid #86efac',
                  borderRadius: '0.5rem',
                  padding: '1rem',
                }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '0.75rem' }}>
                    <span style={{ fontSize: '1.5rem' }}>📄</span>
                    <div>
                      <p style={{ margin: 0, fontWeight: '500', color: '#1f2937' }}>
                        {publication.document.fileName}
                      </p>
                      <p style={{ margin: '0.25rem 0 0 0', fontSize: '0.875rem', color: '#6b7280' }}>
                        Uploaded: {formatDateTime(publication.document.uploadedAt)}
                      </p>
                    </div>
                  </div>
                  <button
                    onClick={handleDownload}
                    disabled={downloading}
                    style={{
                      width: '100%',
                      backgroundColor: downloading ? '#6b7280' : '#10b981',
                      color: 'white',
                      padding: '0.75rem',
                      borderRadius: '0.5rem',
                      border: 'none',
                      cursor: downloading ? 'not-allowed' : 'pointer',
                      fontWeight: '500',
                      marginTop: '0.75rem',
                    }}
                  >
                    {downloading ? 'Downloading...' : '⬇️ Download Document'}
                  </button>
                </div>
              ) : (
                <div
                  style={{
                    backgroundColor: '#f9fafb',
                    border: '1px dashed #d1d5db',
                    borderRadius: '0.5rem',
                    padding: '1.5rem',
                    textAlign: 'center',
                    color: '#6b7280',
                  }}
                >
                  <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>📭</div>
                  <p style={{ margin: 0 }}>No document uploaded yet</p>
                </div>
              )}
            </div>


            {/* Close Button */}
            <button
              onClick={onClose}
              style={{
                width: '100%',
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
                marginTop: '1rem',
              }}
            >
              Close
            </button>
          </div>
        ) : null}
      </div>
    </div>
  );
}
