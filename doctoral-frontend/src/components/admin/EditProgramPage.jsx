import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function EditProgramPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [program, setProgram] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    scientificArea: '',
    faculty: '',
    availableSlots: 0,
    tuitionFee: 0,
    internationalTuitionFee: 0,
    specialRequirements: '',
    isActive: true,
  });

  useEffect(() => {
    fetchProgram();
  }, [id]);

  const fetchProgram = async () => {
    try {
      setLoading(true);
      const programData = await studentService.getProgramById(id);
      
      setProgram(programData);
      setFormData({
        name: programData.name || '',
        scientificArea: programData.scientificArea || '',
        faculty: programData.faculty || '',
        availableSlots: programData.availableSlots || 0,
        tuitionFee: programData.tuitionFee || 0,
        internationalTuitionFee: programData.internationalTuitionFee || 0,
        specialRequirements: programData.specialRequirements || '',
        isActive: programData.isActive !== undefined ? programData.isActive : true,
      });
    } catch (err) {
      console.error('Failed to fetch program:', err);
      setError('Failed to load program data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : type === 'number' ? parseFloat(value) : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      setError('Program name is required');
      return;
    }

    if (!formData.scientificArea.trim()) {
      setError('Scientific area is required');
      return;
    }

    if (!formData.faculty.trim()) {
      setError('Faculty is required');
      return;
    }

    if (formData.availableSlots < 0) {
      setError('Available slots cannot be negative');
      return;
    }

    if (formData.tuitionFee < 0 || formData.internationalTuitionFee < 0) {
      setError('Tuition fees cannot be negative');
      return;
    }

    try {
      setSaving(true);
      setError('');
      await studentService.updateProgram(id, formData);
      alert('Program updated successfully');
      navigate('/admin/programs');
    } catch (err) {
      console.error('Failed to update program:', err);
      setError(err.response?.data?.message || 'Failed to update program');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Loading program data...</p>
      </div>
    );
  }

  if (!program) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Program not found</p>
        <Link to="/admin/programs" style={{ color: '#0d9488' }}>
          Back to Programs
        </Link>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin/programs" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Programs
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Edit Program
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Update program information
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
            {/* Program Name */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Program Name *
              </label>
              <input
                type="text"
                name="name"
                value={formData.name}
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

            {/* Scientific Area */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Scientific Area *
              </label>
              <input
                type="text"
                name="scientificArea"
                value={formData.scientificArea}
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

            {/* Faculty */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Faculty *
              </label>
              <input
                type="text"
                name="faculty"
                value={formData.faculty}
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

            {/* Available Slots */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Available Slots *
              </label>
              <input
                type="number"
                name="availableSlots"
                min="0"
                value={formData.availableSlots}
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

            {/* Tuition Fee */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Tuition Fee (Domestic) *
              </label>
              <input
                type="number"
                name="tuitionFee"
                min="0"
                step="0.01"
                value={formData.tuitionFee}
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

            {/* International Tuition Fee */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Tuition Fee (International) *
              </label>
              <input
                type="number"
                name="internationalTuitionFee"
                min="0"
                step="0.01"
                value={formData.internationalTuitionFee}
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

            {/* Special Requirements */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
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
                  fontSize: '0.875rem',
                  fontFamily: 'inherit',
                  boxSizing: 'border-box',
                  resize: 'vertical',
                }}
              />
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
                Active Program
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
                to="/admin/programs"
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
