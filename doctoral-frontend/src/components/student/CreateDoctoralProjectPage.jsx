import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import { useAuth } from '../../context/AuthContext';

export default function CreateDoctoralProjectPage() {
  const [step, setStep] = useState(1); // 1: Basic Info, 2: Upload Proposal, 3: Review & Submit
  const [programs, setPrograms] = useState([]);
  const [mentors, setMentors] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [projectId, setProjectId] = useState(null);
  const [submitted, setSubmitted] = useState(false);

  // Form state - Step 1
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [selectedProgram, setSelectedProgram] = useState('');
  const [selectedMentor, setSelectedMentor] = useState('');

  // Form state - Step 2
  const [proposalFile, setProposalFile] = useState(null);
  const [uploading, setUploading] = useState(false);
  const [uploadStatus, setUploadStatus] = useState(null);

  const navigate = useNavigate();
  const { user } = useAuth();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [programsData, mentorsData] = await Promise.all([
        studentService.getAllPrograms(),
        studentService.getAllMentors(),
      ]);
      setPrograms(programsData || []);
      setMentors(mentorsData || []);
    } catch (err) {
      setError('Failed to load programs and mentors');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateDraft = async (e) => {
    e.preventDefault();
  
    if (!title.trim()) {
        setError('Please enter a project title');
        return;
    }
    if (!selectedMentor) {
        setError('Please select a mentor');
        return;
    }

    setError('');
    setLoading(true);

    try {
        const profile = await studentService.getStudentProfile();
        const studentId = profile.id || profile.studentId;
    
        const createResult = await studentService.createDoctoralProjectDraft({
            studentId: studentId,        
            mentorId: parseInt(selectedMentor),  
            title: title.trim(),
            researchArea: description.trim(),    
        });

        if (createResult.id) {
            setProjectId(createResult.id);
            setStep(2);
            setError('');
        } else {
            setError(createResult.message || 'Failed to create doctoral project draft');
        }
        } catch (err) {
            const errorMsg = err.response?.data?.message || err.message || 'Failed to create project';
            setError(errorMsg);
            console.error('Error creating project:', err);
        } finally {
            setLoading(false);
        }
    };

  const handleFileChange = (e) => {
    const file = e.target.files[0]; // 
    if (file) {
        if (file.size > 10 * 1024 * 1024) {
            setError('File size must be less than 10MB');
            return;
        }
        setProposalFile(file); // 
        setError('');
        }
    };


  const handleUploadProposal = async () => {
    if (!proposalFile) {
        setError('Please select a proposal document');
        return;
    }

    if (!projectId) {
        setError('Project ID not found. Please try again.');
        return;
    }

    setUploading(true);
    setError('');

    try {
        console.log('Uploading with:', {
            projectId,
            fileName: proposalFile.name,
            documentType: 4,
        });

        const result = await studentService.uploadDoctoralProjectProposal(
            projectId,
            proposalFile,
            4  
        );

        if (result.success) {
            setUploadStatus('success');
            setError('');
            setTimeout(() => {
                setStep(3);
            }, 500);
        } else {
            setError(result.message || 'Failed to upload proposal');
            setUploadStatus('error');
        }
    } catch (err) {
        const errorMsg = err.response?.data?.message || err.message || 'Upload failed';
        setError(errorMsg);
        setUploadStatus('error');
        console.error('Error uploading proposal:', err);
     } finally {
        setUploading(false);
    }
    };



  const handleSubmit = async () => {
    setLoading(true);
    setError('');

    try {
      if (!projectId) {
        setError('Project ID not found. Please try again.');
        setLoading(false);
        return;
      }

      console.log('Submitting doctoral project with ID:', projectId);
      const result = await studentService.submitDoctoralProject(projectId);

      if (result.id) {
        setSubmitted(true);
        alert('Doctoral project submitted successfully!');
        // Redirect to projects page after a moment
        setTimeout(() => {
          navigate('/doctoral-project');
        }, 1500);
      } else {
        setError(result.message || 'Failed to submit project');
      }
    } catch (err) {
      const msg = err.response?.data?.message || err.message || 'Failed to submit';
      setError(msg);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading && step === 1) {
    return <div style={{ padding: '2rem', textAlign: 'center' }}>Loading...</div>;
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '600px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/doctoral-project" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Projects
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            {step === 1 ? 'Create Doctoral Project' : step === 2 ? 'Upload Proposal' : 'Review & Submit'}
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            {step === 1
              ? 'Step 1 of 3: Basic project information'
              : step === 2
              ? 'Step 2 of 3: Upload your proposal document'
              : 'Step 3 of 3: Review and submit'}
          </p>
        </div>

        {/* Error Alert */}
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

        {/* Step 1: Basic Project Information */}
        {step === 1 && (
          <form onSubmit={handleCreateDraft} style={{
            backgroundColor: 'white',
            borderRadius: '0.5rem',
            padding: '1.5rem',
            border: '1px solid #e5e7eb',
          }}>
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                Project Title *
              </label>
              <input
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Enter your project title"
                style={{
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                  boxSizing: 'border-box',
                }}
                required
              />
            </div>

            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                Research Area / Description
              </label>
              <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Describe your doctoral research project"
                style={{
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                  minHeight: '120px',
                  boxSizing: 'border-box',
                  fontFamily: 'inherit',
                }}
              />
            </div>

            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                Preferred Mentor *
              </label>
              <select
                value={selectedMentor}
                onChange={(e) => setSelectedMentor(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                }}
                required
              >
                <option value="">Select a mentor...</option>
                {mentors.map((mentor) => (
                  <option key={mentor.id} value={mentor.id}>
                    {mentor.fullName} - {mentor.title}
                  </option>
                ))}
              </select>
            </div>

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
              }}
            >
              {loading ? 'Creating...' : 'Next: Upload Proposal'}
            </button>
          </form>
        )}

        {/* Step 2: Upload Proposal Document */}
        {step === 2 && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.5rem',
            padding: '1.5rem',
            border: '1px solid #e5e7eb',
          }}>
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                Proposal Document *
              </label>
              <input
                type="file"
                onChange={handleFileChange}
                accept=".pdf,.doc,.docx"
                style={{
                  display: 'block',
                  marginBottom: '0.5rem',
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                }}
              />
              <p style={{
                fontSize: '0.75rem',
                color: '#6b7280',
                margin: '0.5rem 0 0 0',
              }}>
                Accepted formats: PDF, DOC, DOCX (Max 10MB)
              </p>
            </div>

            {proposalFile && (
              <div style={{
                backgroundColor: '#f0fdf4',
                border: '1px solid #dcfce7',
                padding: '1rem',
                borderRadius: '0.5rem',
                marginBottom: '1.5rem',
              }}>
                <p style={{
                  fontSize: '0.875rem',
                  color: '#166534',
                  margin: 0,
                }}>
                  ✓ Selected: <strong>{proposalFile.name}</strong>
                </p>
              </div>
            )}

            <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem' }}>
              <button
                onClick={() => setStep(1)}
                style={{
                  flex: 1,
                  background: '#e5e7eb',
                  color: '#1f2937',
                  padding: '0.75rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: 'pointer',
                  fontWeight: '600',
                }}
              >
                Back
              </button>
              <button
                onClick={handleUploadProposal}
                disabled={!proposalFile || uploading}
                style={{
                  flex: 1,
                  background: uploadStatus === 'success' ? '#10b981' : '#3b82f6',
                  color: 'white',
                  padding: '0.75rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: !proposalFile || uploading ? 'not-allowed' : 'pointer',
                  fontWeight: '600',
                  opacity: !proposalFile || uploading ? 0.5 : 1,
                }}
              >
                {uploading ? 'Uploading...' : uploadStatus === 'success' ? '✓ Uploaded' : 'Upload Document'}
              </button>
            </div>

            {uploadStatus === 'success' && (
              <div style={{
                backgroundColor: '#f0fdf4',
                border: '1px solid #dcfce7',
                color: '#166534',
                padding: '1rem',
                borderRadius: '0.5rem',
                textAlign: 'center',
              }}>
                <p style={{ margin: 0 }}>
                  Document uploaded successfully! Proceeding to review...
                </p>
              </div>
            )}
          </div>
        )}

        {/* Step 3: Review & Submit */}
        {step === 3 && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.5rem',
            padding: '1.5rem',
            border: '1px solid #e5e7eb',
          }}>
            <div style={{
              backgroundColor: '#f0fdf4',
              border: '1px solid #dcfce7',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1.5rem',
            }}>
              <h3 style={{ color: '#166534', margin: '0 0 0.5rem 0' }}>
                ✓ Ready to Submit
              </h3>
              <p style={{ color: '#166534', margin: 0 }}>
                Your project is ready for submission. Click submit to complete the process.
              </p>
            </div>

            <div style={{
              backgroundColor: '#f9fafb',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1.5rem',
            }}>
              <h4 style={{ color: '#1f2937', marginBottom: '0.5rem' }}>Project Summary</h4>
              <p style={{ color: '#6b7280', margin: '0 0 0.5rem 0' }}>
                <strong>Title:</strong> {title}
              </p>
              <p style={{ color: '#6b7280', margin: '0 0 0.5rem 0' }}>
                <strong>Research Area:</strong> {description}
              </p>
              <p style={{ color: '#6b7280', margin: 0 }}>
                <strong>Mentor:</strong> {mentors.find(m => m.id === parseInt(selectedMentor))?.fullName || 'N/A'}
              </p>
            </div>

            <div style={{ display: 'flex', gap: '1rem' }}>
              <button
                onClick={() => setStep(2)}
                style={{
                  flex: 1,
                  background: '#e5e7eb',
                  color: '#1f2937',
                  padding: '0.75rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: 'pointer',
                  fontWeight: '600',
                }}
              >
                Back
              </button>
              <button
                onClick={handleSubmit}
                disabled={loading}
                style={{
                  flex: 1,
                  background: 'linear-gradient(90deg, #10b981 0%, #059669 100%)',
                  color: 'white',
                  padding: '0.75rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: loading ? 'not-allowed' : 'pointer',
                  fontWeight: '600',
                  opacity: loading ? 0.5 : 1,
                }}
              >
                {loading ? 'Submitting...' : 'Submit Project'}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
