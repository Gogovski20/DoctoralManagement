import React, { useState, useEffect } from 'react';
import { studentService } from '../../api/studentService';

export default function EditPublicationModal({ publicationId, onClose, onSuccess }) {
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    id: publicationId,
    title: '',
    journal: '',
    publishedOn: '',
    doi: '',
    isIndexedInScopus: false,
    isIndexedInThomsonReuters: false,
  });

  useEffect(() => {
    const fetchPublication = async () => {
      try {
        setLoading(true);
        setError('');
        const data = await studentService.getPublicationById(publicationId);
        
        if (data.isApproved) {
          setError('This publication has been approved and cannot be edited.');
          return;
        }

        const dateObj = new Date(data.publishedOn);
        const formattedDate = dateObj.toISOString().split('T')[0];

        setFormData({
          id: data.id,
          title: data.title,
          journal: data.journal,
          publishedOn: formattedDate,
          doi: data.doi || '',
          isIndexedInScopus: data.isIndexedInScopus || false,
          isIndexedInThomsonReuters: data.isIndexedInThomsonReuters || false,
        });
      } catch (err) {
        setError(`Failed to load publication: ${err.response?.data?.message || err.message || 'Unknown error'}`);
      } finally {
        setLoading(false);
      }
    };

    fetchPublication();
  }, [publicationId]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      setSubmitting(true);
      setError('');

      const payload = {
        id: formData.id,
        title: formData.title,
        journal: formData.journal,
        publishedOn: formData.publishedOn,
        doi: formData.doi,
        isIndexedInScopus: formData.isIndexedInScopus,
        isIndexedInThomsonReuters: formData.isIndexedInThomsonReuters,
      };

      await studentService.updatePublication(payload);
      onSuccess();
      onClose();
    } catch (err) {
      setError(`Failed to update publication: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div style={{
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
      }}>
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '2rem',
          maxWidth: '500px',
          width: '90%',
          textAlign: 'center',
        }}>
          <div style={{ fontSize: '2rem', marginBottom: '1rem' }}>⏳</div>
          <p style={{ color: '#6b7280' }}>Loading publication details...</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{
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
    }}>
      <div style={{
        backgroundColor: 'white',
        borderRadius: '0.75rem',
        boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1)',
        padding: '2rem',
        maxWidth: '500px',
        width: '90%',
        maxHeight: '90vh',
        overflowY: 'auto',
      }}>
        {/* Header */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
          <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Edit Publication
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

        {/* Error banner */}
        {error && (
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
            fontSize: '0.875rem',
          }}>
            {error}
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
          <div>
            <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem', fontSize: '0.875rem' }}>
              Title *
            </label>
            <input
              type="text"
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              required
              placeholder="Publication title"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
            />
          </div>

          <div>
            <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem', fontSize: '0.875rem' }}>
              Journal *
            </label>
            <input
              type="text"
              value={formData.journal}
              onChange={(e) => setFormData({ ...formData, journal: e.target.value })}
              required
              placeholder="Journal name"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            <div>
              <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem', fontSize: '0.875rem' }}>
                Published On *
              </label>
              <input
                type="date"
                value={formData.publishedOn}
                onChange={(e) => setFormData({ ...formData, publishedOn: e.target.value })}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>
            <div>
              <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem', fontSize: '0.875rem' }}>
                DOI
              </label>
              <input
                type="text"
                value={formData.doi}
                onChange={(e) => setFormData({ ...formData, doi: e.target.value })}
                placeholder="e.g., 10.1234/example"
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>
          </div>

          <div style={{ display: 'flex', gap: '1rem', flexDirection: 'column' }}>
            <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: '500', color: '#374151' }}>
              <input
                type="checkbox"
                checked={formData.isIndexedInScopus}
                onChange={(e) => setFormData({ ...formData, isIndexedInScopus: e.target.checked })}
                style={{ cursor: 'pointer' }}
              />
              Indexed in Scopus
            </label>
            <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: '500', color: '#374151' }}>
              <input
                type="checkbox"
                checked={formData.isIndexedInThomsonReuters}
                onChange={(e) => setFormData({ ...formData, isIndexedInThomsonReuters: e.target.checked })}
                style={{ cursor: 'pointer' }}
              />
              Indexed in Thomson Reuters
            </label>
          </div>

          {/* Buttons */}
          <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', marginTop: '1.5rem', borderTop: '1px solid #e5e7eb', paddingTop: '1.5rem' }}>
            <button
              type="button"
              onClick={onClose}
              disabled={submitting}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: submitting ? 'not-allowed' : 'pointer',
                fontWeight: '500',
                opacity: submitting ? 0.6 : 1,
              }}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={submitting}
              style={{
                background: submitting ? '#9ca3af' : 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: submitting ? 'not-allowed' : 'pointer',
                fontWeight: '500',
              }}
            >
              {submitting ? 'Updating...' : 'Update Publication'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}