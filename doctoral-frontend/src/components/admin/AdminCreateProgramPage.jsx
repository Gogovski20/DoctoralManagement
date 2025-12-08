import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminCreateProgramPage() {
  const [formData, setFormData] = useState({
    name: '',
    scientificArea: '',
    faculty: '',
    availableSlots: 10,
    tuitionFee: 0,
    internationalTuitionFee: 0,
    specialRequirements: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'number' ? (value === '' ? '' : parseFloat(value)) : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      const result = await studentService.createProgram(formData);
      if (result.id) {
        setSuccess(`Program "${formData.name}" created successfully!`);
        setFormData({
          name: '',
          scientificArea: '',
          faculty: '',
          availableSlots: 10,
          tuitionFee: 0,
          internationalTuitionFee: 0,
          specialRequirements: '',
        });
        setTimeout(() => navigate('/dashboard'), 2000);
      } else {
        setError(result.message || 'Failed to create program');
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create program');
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
            Create Doctoral Program
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Add a new doctoral program to the system
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
          {/* Program Name */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Program Name *
            </label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Computer Science PhD"
              required
            />
          </div>

          {/* Scientific Area */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Scientific Area *
            </label>
            <input
              type="text"
              name="scientificArea"
              value={formData.scientificArea}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Computer Science, Mathematics"
              required
            />
          </div>

          {/* Faculty */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Faculty *
            </label>
            <input
              type="text"
              name="faculty"
              value={formData.faculty}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="e.g., Faculty of Technical Sciences"
              required
            />
          </div>

          {/* Available Slots */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Available Slots *
            </label>
            <input
              type="number"
              name="availableSlots"
              value={formData.availableSlots}
              onChange={handleChange}
              min="0"
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

          {/* Tuition Fee */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Tuition Fee (€) *
            </label>
            <input
              type="number"
              name="tuitionFee"
              value={formData.tuitionFee}
              onChange={handleChange}
              min="0"
              step="0.01"
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

          {/* International Tuition Fee */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              International Tuition Fee (€) *
            </label>
            <input
              type="number"
              name="internationalTuitionFee"
              value={formData.internationalTuitionFee}
              onChange={handleChange}
              min="0"
              step="0.01"
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

          {/* Special Requirements */}
          <div style={{ marginBottom: '2rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Special Requirements
            </label>
            <textarea
              name="specialRequirements"
              value={formData.specialRequirements}
              onChange={handleChange}
              rows="4"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
                resize: 'vertical',
              }}
              placeholder="Any special requirements for this program..."
            />
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
            {loading ? 'Creating Program...' : 'Create Program'}
          </button>
        </form>
      </div>
    </div>
  );
}