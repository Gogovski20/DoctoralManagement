import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

const DOCUMENT_TYPES = [
  { key: 'MotivationLetter', label: 'Motivation Letter', required: true },
  { key: 'ResearchProposal', label: 'Research Proposal', required: true },
  { key: 'EnglishCertificate', label: 'English Certificate', required: true },
  { key: 'CV', label: 'Curriculum Vitae', required: false },
  { key: 'TranscriptUndergrad', label: 'Undergraduate Transcript', required: false },
  { key: 'TranscriptMasters', label: 'Master Transcript', required: false },
];

export default function UploadDocumentsPage() {
  const { ApplicationId } = useParams();
  const navigate = useNavigate();
  const [files, setFiles] = useState({});
  const [uploading, setUploading] = useState({});
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleFileChange = (docType, file) => {
    if (file) {
      setFiles(prev => ({ ...prev, [docType]: file }));
      setError('');
    }
  };

  const handleUpload = async (docType) => {
    const file = files[docType];
    if (!file) {
      setError(`Please select a file for ${DOCUMENT_TYPES.find(d => d.key === docType)?.label}`);
      return;
    }

    setUploading(prev => ({ ...prev, [docType]: true }));
    setError('');
    setSuccess('');

    try {
      const result = await studentService.uploadApplicationDocument(
        ApplicationId,
        file,
        file.name,
        docType
      );
      
      if (result.success) {
        setSuccess(`${DOCUMENT_TYPES.find(d => d.key === docType)?.label} uploaded successfully!`);
        setFiles(prev => ({ ...prev, [docType]: null }));
        
        // Reset file input
        const fileInput = document.querySelector(`input[data-doctype="${docType}"]`);
        if (fileInput) fileInput.value = '';
      } else {
        setError(`Failed to upload ${docType}: ${result.message}`);
      }
    } catch (err) {
      console.error('Upload error:', err);
      setError(`Failed to upload ${docType}: ${
        err.response?.data?.message || 
        err.response?.data || 
        err.message || 
        'Unknown error'
      }`);
    } finally {
      setUploading(prev => ({ ...prev, [docType]: false }));
    }
  };

  const handleSubmitAll = async () => {
    const requiredDocs = DOCUMENT_TYPES.filter(doc => doc.required);
    const missingDocs = requiredDocs.filter(doc => !files[doc.key]);
    
    if (missingDocs.length > 0) {
      setError(`Please upload all required documents: ${missingDocs.map(d => d.label).join(', ')}`);
      return;
    }

    setError('');
    setSuccess('');

    for (const doc of requiredDocs) {
      if (files[doc.key]) {
        await handleUpload(doc.key);
        await new Promise(resolve => setTimeout(resolve, 500));
      }
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '600px', margin: '0 auto' }}>
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Upload Documents
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Application ID: {ApplicationId}
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

        {success && (
          <div style={{
            backgroundColor: '#f0fdf4',
            border: '1px solid #86efac',
            color: '#166534',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
          }}>
            {success}
          </div>
        )}

        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          {DOCUMENT_TYPES.map((doc) => (
            <div key={doc.key} style={{ marginBottom: '1.5rem', paddingBottom: '1.5rem', borderBottom: '1px solid #e5e7eb' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                {doc.label} {doc.required && <span style={{ color: '#ef4444' }}>*</span>}
              </label>
              
              <input
                type="file"
                data-doctype={doc.key}
                onChange={(e) => handleFileChange(doc.key, e.target.files[0])}
                accept=".pdf,.doc,.docx,.jpg,.jpeg,.png"
                style={{
                  display: 'block',
                  marginBottom: '0.5rem',
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                }}
              />

              {files[doc.key] && (
                <div>
                  <p style={{ fontSize: '0.875rem', color: '#6b7280', marginBottom: '0.5rem' }}>
                    Selected: <strong>{files[doc.key].name}</strong> ({Math.round(files[doc.key].size / 1024)} KB)
                  </p>
                  <button
                    onClick={() => handleUpload(doc.key)}
                    disabled={uploading[doc.key]}
                    style={{
                      backgroundColor: uploading[doc.key] ? '#9ca3af' : '#3b82f6',
                      color: 'white',
                      padding: '0.5rem 1rem',
                      borderRadius: '0.5rem',
                      border: 'none',
                      cursor: uploading[doc.key] ? 'not-allowed' : 'pointer',
                      fontSize: '0.875rem',
                    }}
                  >
                    {uploading[doc.key] ? 'Uploading...' : 'Upload'}
                  </button>
                </div>
              )}
            </div>
          ))}

          <div style={{ display: 'flex', gap: '1rem', marginTop: '2rem' }}>
            <button
              onClick={() => navigate(`/applications/${ApplicationId}`)}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
              }}
            >
              Back to Application
            </button>
            <button
              onClick={handleSubmitAll}
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
              }}
            >
              Upload All Required Documents
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}