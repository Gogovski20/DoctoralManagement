import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function EditApplicationPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [preferredMentorId, setPreferredMentorId] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        setError('');
        const app = await studentService.getApplicationById(id);
        setPreferredMentorId(app.preferredMentorId || app.prefferedMentorId || '');
      } catch (err) {
        console.error('Failed to load application:', err);
        setError(err.response?.data?.message || err.message || 'Failed to load application');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setSaving(true);
      setError('');
      await studentService.updateApplication(Number(id), {
        preferredMentorId: preferredMentorId ? Number(preferredMentorId) : null,
      });
      navigate('/dashboard');
    } catch (err) {
      console.error('Failed to update application:', err);
      setError(err.response?.data?.message || err.message || 'Failed to update application');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#6b7280' }}>
        Loading application...
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '600px', margin: '0 auto' }}>
        <div style={{ marginBottom: '1.5rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h1 style={{ fontSize: '1.75rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Edit Application #{id}
          </h1>
          <Link
            to="/dashboard"
            style={{
              textDecoration: 'none',
              backgroundColor: '#e5e7eb',
              color: '#1f2937',
              padding: '0.5rem 1rem',
              borderRadius: '0.5rem',
              fontWeight: '500',
              fontSize: '0.875rem',
            }}
          >
            ← Back
          </Link>
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

        <form onSubmit={handleSubmit} style={{ backgroundColor: 'white', padding: '1.5rem', borderRadius: '0.75rem', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', border: '1px solid #e5e7eb' }}>
          <div style={{ marginBottom: '1.25rem' }}>
            <label
              style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}
            >
              Preferred Mentor ID
            </label>
            <input
              type="number"
              value={preferredMentorId}
              onChange={(e) => setPreferredMentorId(e.target.value)}
              placeholder="Enter mentor ID (optional)"
              style={{
                width: '100%',
                padding: '0.75rem',
                borderRadius: '0.5rem',
                border: '1px solid #d1d5db',
                fontSize: '0.95rem',
              }}
            />
            <p style={{ fontSize: '0.75rem', color: '#6b7280', marginTop: '0.25rem' }}>
              You can update only the preferred mentor while the application is in Draft or Submitted status.
            </p>
          </div>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem' }}>
            <button
              type="button"
              onClick={() => navigate('/dashboard')}
              disabled={saving}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: saving ? 'not-allowed' : 'pointer',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              style={{
                backgroundColor: saving ? '#6b7280' : '#10b981',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: saving ? 'not-allowed' : 'pointer',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}
            >
              {saving ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
