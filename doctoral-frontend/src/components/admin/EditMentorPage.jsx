import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function EditMentorPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [mentor, setMentor] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);
  const [researchAreaInput, setResearchAreaInput] = useState('');
  const [formData, setFormData] = useState({
    fullName: '',
    department: '',
    title: '',
    maxStudents: 5,
    isActive: true,
    researchAreas: [],
  });

  useEffect(() => {
    fetchMentor();
  }, [id]);

  const fetchMentor = async () => {
    try {
      setLoading(true);
      const mentorData = await studentService.getMentorById(id);
      
      setMentor(mentorData);
      setFormData({
        fullName: mentorData.fullName || '',
        department: mentorData.department || '',
        title: mentorData.title || '',
        maxStudents: mentorData.maxStudents || 5,
        isActive: mentorData.isActive !== undefined ? mentorData.isActive : true,
        researchAreas: Array.isArray(mentorData.researchAreas) ? mentorData.researchAreas : [],
      });
    } catch (err) {
      console.error('Failed to fetch mentor:', err);
      setError('Failed to load mentor data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : type === 'number' ? parseInt(value) : value,
    });
  };

  const handleAddResearchArea = () => {
    if (researchAreaInput.trim() && !formData.researchAreas.includes(researchAreaInput.trim())) {
      setFormData({
        ...formData,
        researchAreas: [...formData.researchAreas, researchAreaInput.trim()],
      });
      setResearchAreaInput('');
    }
  };

  const handleRemoveResearchArea = (index) => {
    setFormData({
      ...formData,
      researchAreas: formData.researchAreas.filter((_, i) => i !== index),
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!formData.fullName.trim()) {
      setError('Full name is required');
      return;
    }

    if (!formData.department.trim()) {
      setError('Department is required');
      return;
    }

    if (!formData.title.trim()) {
      setError('Title is required');
      return;
    }

    if (formData.maxStudents <= 0 || formData.maxStudents > 50) {
      setError('Max students must be between 1 and 50');
      return;
    }

    try {
      setSaving(true);
      setError('');
      await studentService.updateMentor(id, formData);
      alert('Mentor updated successfully');
      navigate('/admin/mentors');
    } catch (err) {
      console.error('Failed to update mentor:', err);
      setError(err.response?.data?.message || 'Failed to update mentor');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Loading mentor data...</p>
      </div>
    );
  }

  if (!mentor) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Mentor not found</p>
        <Link to="/admin/mentors" style={{ color: '#0d9488' }}>
          Back to Mentors
        </Link>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin/mentors" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Mentors
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Edit Mentor
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Update mentor information
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

        {/* Form Card */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <form onSubmit={handleSubmit}>
            {/* Full Name */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Full Name *
              </label>
              <input
                type="text"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Department */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Department *
              </label>
              <input
                type="text"
                name="department"
                value={formData.department}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Title */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Title *
              </label>
              <input
                type="text"
                name="title"
                value={formData.title}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Max Students */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Max Students (1-50) *
              </label>
              <input
                type="number"
                name="maxStudents"
                min="1"
                max="50"
                value={formData.maxStudents}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Research Areas */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Research Areas
              </label>
              <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.75rem' }}>
                <input
                  type="text"
                  value={researchAreaInput}
                  onChange={(e) => setResearchAreaInput(e.target.value)}
                  onKeyPress={(e) => {
                    if (e.key === 'Enter') {
                      e.preventDefault();
                      handleAddResearchArea();
                    }
                  }}
                  placeholder="Add a research area and press Enter"
                  style={{
                    flex: 1,
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '0.875rem',
                    boxSizing: 'border-box',
                  }}
                />
                <button
                  type="button"
                  onClick={handleAddResearchArea}
                  style={{
                    backgroundColor: '#0d9488',
                    color: 'white',
                    padding: '0.75rem 1rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: 'pointer',
                    fontWeight: '500',
                    fontSize: '0.875rem',
                  }}
                >
                  Add
                </button>
              </div>

              {/* Display Research Areas */}
              {formData.researchAreas.length > 0 && (
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '0.5rem' }}>
                  {formData.researchAreas.map((area, index) => (
                    <span
                      key={index}
                      style={{
                        display: 'inline-flex',
                        alignItems: 'center',
                        backgroundColor: '#d1f2eb',
                        color: '#0d5a47',
                        padding: '0.5rem 0.75rem',
                        borderRadius: '9999px',
                        fontSize: '0.875rem',
                        fontWeight: '500',
                      }}
                    >
                      {area}
                      <button
                        type="button"
                        onClick={() => handleRemoveResearchArea(index)}
                        style={{
                          marginLeft: '0.5rem',
                          background: 'none',
                          border: 'none',
                          color: '#0d5a47',
                          cursor: 'pointer',
                          fontSize: '1rem',
                          padding: 0,
                        }}
                      >
                        ×
                      </button>
                    </span>
                  ))}
                </div>
              )}
            </div>

            {/* Active Status */}
            <div style={{ marginBottom: '1.5rem', display: 'flex', alignItems: 'center' }}>
              <input
                type="checkbox"
                name="isActive"
                checked={formData.isActive}
                onChange={handleChange}
                style={{
                  width: '1.25rem',
                  height: '1.25rem',
                  cursor: 'pointer',
                  marginRight: '0.75rem',
                }}
              />
              <label style={{ fontSize: '0.875rem', fontWeight: '500', color: '#374151', cursor: 'pointer' }}>
                Active Mentor
              </label>
            </div>

            {/* Submit Button */}
            <div style={{ display: 'flex', gap: '1rem' }}>
              <button
                type="submit"
                disabled={saving}
                style={{
                  backgroundColor: '#0d9488',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: saving ? 'not-allowed' : 'pointer',
                  fontWeight: '500',
                  opacity: saving ? 0.5 : 1,
                }}
              >
                {saving ? 'Saving...' : 'Save Changes'}
              </button>
              <Link
                to="/admin/mentors"
                style={{
                  backgroundColor: '#e5e7eb',
                  color: '#1f2937',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  textDecoration: 'none',
                  fontWeight: '500',
                  display: 'inline-flex',
                  alignItems: 'center',
                }}
              >
                Cancel
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
