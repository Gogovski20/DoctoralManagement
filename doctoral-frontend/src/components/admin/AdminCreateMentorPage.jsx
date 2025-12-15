import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminCreateMentorPage() {
  const [formData, setFormData] = useState({
    fullName: '',
    department: '',
    email: '',
    title: '',
    maxStudents: 5,
    researchAreas: [''],
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'number' ? (value === '' ? '' : parseInt(value)) : value,
    }));
  };

  const handleResearchAreaChange = (index, value) => {
    const newResearchAreas = [...formData.researchAreas];
    newResearchAreas[index] = value;
    setFormData(prev => ({
      ...prev,
      researchAreas: newResearchAreas,
    }));
  };

  const addResearchArea = () => {
    setFormData(prev => ({
      ...prev,
      researchAreas: [...prev.researchAreas, ''],
    }));
  };

  const removeResearchArea = (index) => {
    if (formData.researchAreas.length > 1) {
      const newResearchAreas = formData.researchAreas.filter((_, i) => i !== index);
      setFormData(prev => ({
        ...prev,
        researchAreas: newResearchAreas,
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    const filteredResearchAreas = formData.researchAreas.filter(area => area.trim() !== '');

    if (filteredResearchAreas.length === 0) {
      setError('Please add at least one research area');
      setLoading(false);
      return;
    }

    try {
      const mentorData = {
        ...formData,
        researchAreas: filteredResearchAreas,
      };

      const result = await studentService.createMentor(mentorData);
      if (result.id) {
        setSuccess(`Mentor "${formData.fullName}" created successfully!`);
        setFormData({
          fullName: '',
          department: '',
          email: '',
          title: '',
          maxStudents: 5,
          researchAreas: [''],
        });
        setTimeout(() => navigate('/dashboard'), 2000);
      } else {
        setError(result.message || 'Failed to create mentor');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create mentor');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '700px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Create New Mentor
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Add a new mentor to the system
          </p>
        </div>

        {/* Messages */}
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
            padding: '1.5rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
            textAlign: 'center',
          }}>
            <p style={{ margin: '0 0 1rem 0', fontSize: '1rem' }}>
              ✅ {success}
            </p>
            <button
              onClick={() => navigate('/dashboard')}
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '600',
                fontSize: '0.875rem',
              }}
              onMouseEnter={(e) => {
                e.currentTarget.style.backgroundColor = '#0f766e';
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = '#0d9488';
              }}
            >
              Back to Admin Dashboard
            </button>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '2rem',
          border: '1px solid #e5e7eb',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
        }}>
          {/* Full Name */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Full Name *
            </label>
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Dr. John Smith"
              required
            />
          </div>

          {/* Title */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Title *
            </label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Professor, Associate Professor"
              required
            />
          </div>

          {/* Email */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Email *
            </label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., john.smith@university.edu"
              required
            />
          </div>

          {/* Department */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Department *
            </label>
            <input
              type="text"
              name="department"
              value={formData.department}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Department of Computer Science"
              required
            />
          </div>

          {/* Max Students */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Maximum Students *
            </label>
            <input
              type="number"
              name="maxStudents"
              value={formData.maxStudents}
              onChange={handleChange}
              min="1"
              max="50"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              required
            />
          </div>

          {/* Research Areas */}
          <div style={{ marginBottom: '2rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Research Areas *
            </label>
            {formData.researchAreas.map((area, index) => (
              <div key={index} style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.5rem' }}>
                <input
                  type="text"
                  value={area}
                  onChange={(e) => handleResearchAreaChange(index, e.target.value)}
                  style={{
                    flex: 1,
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    boxSizing: 'border-box',
                  }}
                  placeholder="e.g., Artificial Intelligence, Machine Learning"
                  required={index === 0}
                />
                {formData.researchAreas.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeResearchArea(index)}
                    style={{
                      backgroundColor: '#ef4444',
                      color: 'white',
                      padding: '0.75rem 1rem',
                      borderRadius: '0.5rem',
                      border: 'none',
                      cursor: 'pointer',
                      fontSize: '0.875rem',
                    }}
                  >
                    Remove
                  </button>
                )}
              </div>
            ))}
            <button
              type="button"
              onClick={addResearchArea}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.5rem 1rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontSize: '0.875rem',
                marginTop: '0.5rem',
              }}
            >
              + Add Another Research Area
            </button>
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={loading}
            style={{
              width: '100%',
              background: 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
              color: 'white',
              fontWeight: '600',
              padding: '0.75rem',
              borderRadius: '0.5rem',
              border: 'none',
              cursor: loading ? 'not-allowed' : 'pointer',
              opacity: loading ? 0.5 : 1,
              fontSize: '1rem',
            }}
          >
            {loading ? 'Creating Mentor...' : 'Create Mentor'}
          </button>
        </form>
      </div>
    </div>
  );
}