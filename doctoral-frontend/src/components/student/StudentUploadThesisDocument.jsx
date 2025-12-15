import React, { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function StudentUploadThesisDocument() {
  const { projectId } = useParams();
  const navigate = useNavigate();
  const [file, setFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!file) {
      setError('Please select a file.');
      return;
    }

    try {
      setLoading(true);
      setError('');

      const formData = new FormData();
      formData.append('file', file);
      formData.append('type', 'DefenseThesisDocument');

      await studentService.uploadThesisDocument(projectId, formData);

      setSuccess('Thesis document uploaded successfully.');
      setTimeout(() => navigate('/doctoral-projects'), 1500);
    } catch (err) {
      setError(err.response?.data?.message || 'Upload failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '600px', margin: '3rem auto', padding: '2rem', background: 'white', borderRadius: '0.75rem', border: '1px solid #e5e7eb' }}>
      <Link to="/doctoral-projects" style={{ color: '#0d9488' }}>← Back</Link>

      <h2 style={{ marginTop: '1rem' }}>Upload Thesis Document</h2>
      <p style={{ color: '#6b7280', marginBottom: '1rem' }}>
        Upload final thesis (PDF, max 5MB)
      </p>

      {error && <p style={{ color: '#b91c1c' }}>{error}</p>}
      {success && <p style={{ color: '#166534' }}>{success}</p>}

      <form onSubmit={handleSubmit}>
        <input
          type="file"
          accept=".pdf"
          onChange={(e) => setFile(e.target.files[0])}
          style={{ marginBottom: '1rem' }}
        />

        <button
          type="submit"
          disabled={loading}
          style={{
            width: '100%',
            backgroundColor: '#7c3aed',
            color: 'white',
            padding: '0.75rem',
            borderRadius: '0.5rem',
            border: 'none',
            fontWeight: '500',
          }}
        >
          {loading ? 'Uploading...' : 'Upload Thesis'}
        </button>
      </form>
    </div>
  );
}
