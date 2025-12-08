import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import { useAuth } from '../../context/AuthContext';

function getStudentIdFromToken(token) {
  if (!token) return null;
  
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    
    const decoded = JSON.parse(atob(parts[1]));
    
    // Log all claims to debug
    console.log('All JWT Claims:', decoded);
    
    // The JWT only contains user ID, not student ID
    // We'll need to fetch student info from profile
    return null;  // Return null to trigger profile fetch
  } catch (err) {
    console.error('Failed to decode token:', err);
    return null;
  }
}

export default function CreateApplicationPage() {
  const [step, setStep] = useState(1); // 1: Select Program, 2: Upload Docs
  const [programs, setPrograms] = useState([]);
  const [mentors, setMentors] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [applicationId, setApplicationId] = useState(null);
  const [submitted, setSubmitted] = useState(false);


  // Form state - Step 1
  const [selectedProgram, setSelectedProgram] = useState('');
  const [selectedMentor, setSelectedMentor] = useState('');

  // Form state - Step 2
  const [documents, setDocuments] = useState({
    // CV: null,
    // TranscriptUndergrad: null,
    // TranscriptMasters: null,
    MotivationLetter: null,
    EnglishCertificate: null,
    ResearchProposal: null,
  });

  const [uploadStatus, setUploadStatus] = useState({});

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
    if (!selectedProgram) {
      setError('Please select a program');
      return;
    }

    setError('');
    setLoading(true);

    try {
      // Get current user's profile to get studentId
      const profile = await studentService.getStudentProfile();
      const studentId = profile?.studentId || profile?.studentId;
    
      if (!studentId) {
        setError('Unable to determine your student ID. Please contact support.');
        setLoading(false);
        return;
      }

      console.log('Creating application for student:', studentId);
    
      const createResult = await studentService.createApplicationDraft({
        studentId: parseInt(studentId),
        doctoralProgramId: parseInt(selectedProgram),
        preferredMentorId: selectedMentor ? parseInt(selectedMentor) : null,
      });

      if (createResult.id) {
        setApplicationId(createResult.id);
        setStep(2);
        alert('Application draft created successfully! Please upload your documents.');
      } else {
        setError(createResult.message || 'Failed to create application draft');
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.message || 'Failed to create application';
      setError(errorMsg);
      console.error('Error creating application:', err);
    } finally {
      setLoading(false);
    }
  };


  const handleFileChange = (e, docType) => {
    const file = e.target.files[0];
    if (file) {
      setDocuments((prev) => ({
        ...prev,
        [docType]: file,
      }));
    }
  };

  const handleUploadDocument = async (docType) => {
    if (!documents[docType]) {
      setError(`Please select a file for ${docType}`);
      return;
    }

    setUploadStatus(prev => ({ ...prev, [docType]: 'uploading' }));
    setError('');

    try {
      // Create application on first document upload if it doesn't exist yet
      let appId = applicationId;
      if (!appId) {
        let studentId = null;
        
        try {
          const profile = await studentService.getStudentProfile();
          studentId = profile?.studentId;
          console.log('Got student ID from profile:', studentId);
        } catch (profileErr) {
          console.error('Could not fetch profile:', profileErr);
          setError('Unable to determine your student ID. Please try logging in again.');
          setUploadStatus(prev => ({ ...prev, [docType]: 'error' }));
          return;
        }
        
        if (!studentId) {
          setError('Unable to find your student record. Contact support.');
          setUploadStatus(prev => ({ ...prev, [docType]: 'error' }));
          return;
        }

        console.log('Creating application with studentId:', studentId);
        
        const createResult = await studentService.createApplicationDraft(
          studentId,
          parseInt(selectedProgram),
          selectedMentor ? parseInt(selectedMentor) : null
        );
        
        if (!createResult.id) {
          setError(createResult.message || 'Failed to create application');
          setUploadStatus(prev => ({ ...prev, [docType]: 'error' }));
          return;
        }
        
        appId = createResult.id;
        setApplicationId(appId);
        console.log('Application ID set to:', appId);
      }

      console.log(`Uploading ${docType} to application ${appId}`);
      
      const result = await studentService.uploadApplicationDocument(
        appId,
        documents[docType],
        documents[docType].name,
        docType
      );

      if (result.success) {
        setUploadStatus(prev => ({ ...prev, [docType]: 'success' }));
      } else {
        setError(`${docType}: ${result.message || 'Upload failed'}`);
        setUploadStatus(prev => ({ ...prev, [docType]: 'error' }));
      }
    } catch (err) {
      const errorMsg = err.response?.data?.message || err.message || 'Upload failed';
      setError(`${docType}: ${errorMsg}`);
      console.error(`Error uploading ${docType}:`, err);
      setUploadStatus(prev => ({ ...prev, [docType]: 'error' }));
    }
  };

  const handleSubmit = async () => {
    setLoading(true);
    setError('');

    try {
      if (!applicationId) {
        setError('Application ID not found. Please try uploading documents again.');
        setLoading(false);
        return;
      }

      console.log('Submitting application with ID:', applicationId);
      const result = await studentService.submitApplication(applicationId);

      if (result.success || result.id) {
        setSubmitted(true);   // <- flag to show dashboard button
        alert('Application submitted successfully!');
      } else {
        setError(result.message || 'Failed to submit application');
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
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            {step === 1 ? 'Create Application' : 'Upload Documents'}
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            {step === 1
              ? 'Step 1 of 2: Select a doctoral program'
              : 'Step 2 of 2: Upload required documents'}
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

        {/* Step 1: Select Program */}
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
                Doctoral Program *
              </label>
              <select
                value={selectedProgram}
                onChange={(e) => setSelectedProgram(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.5rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                }}
                required
              >
                <option value="">Select a program...</option>
                {programs.map((program) => (
                  <option key={program.id} value={program.id}>
                    {program.name} - {program.scientificArea}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{
                display: 'block',
                fontSize: '0.875rem',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '0.5rem',
              }}>
                Preferred Mentor (Optional)
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
              {loading ? 'Creating...' : 'Next: Upload Documents'}
            </button>
          </form>
        )}

        {/* Step 2: Upload Documents */}
        {step === 2 && (
          <div>
            <div style={{
              backgroundColor: 'white',
              borderRadius: '0.5rem',
              padding: '1.5rem',
              border: '1px solid #e5e7eb',
            }}>
              {Object.keys(documents).map((docType) => (
                <div key={docType} style={{ marginBottom: '1.5rem', paddingBottom: '1.5rem', borderBottom: '1px solid #e5e7eb' }}>
                  <label style={{
                    display: 'block',
                    fontSize: '0.875rem',
                    fontWeight: '500',
                    color: '#374151',
                    marginBottom: '0.5rem',
                  }}>
                    {docType.replace(/([A-Z])/g, ' $1').trim()} *
                  </label>
                  <input
                    type="file"
                    onChange={(e) => handleFileChange(e, docType)}
                    style={{
                      display: 'block',
                      marginBottom: '0.5rem',
                      width: '100%',
                      padding: '0.5rem',
                      border: '1px solid #d1d5db',
                      borderRadius: '0.5rem',
                    }}
                    accept=".pdf,.doc,.docx"
                  />
                  {documents[docType] && (
                    <div>
                      <p style={{
                        fontSize: '0.875rem',
                        color: '#6b7280',
                        marginBottom: '0.5rem',
                      }}>
                        Selected: <strong>{documents[docType].name}</strong>
                      </p>
                      <button
                        onClick={() => handleUploadDocument(docType)}
                        disabled={uploadStatus[docType] === 'uploading' || uploadStatus[docType] === 'success'}
                        style={{
                          background: uploadStatus[docType] === 'success' ? '#10b981' : '#3b82f6',
                          color: 'white',
                          padding: '0.5rem 1rem',
                          borderRadius: '0.5rem',
                          border: 'none',
                          cursor: uploadStatus[docType] === 'uploading' || uploadStatus[docType] === 'success' ? 'not-allowed' : 'pointer',
                          fontSize: '0.875rem',
                          opacity: uploadStatus[docType] === 'uploading' || uploadStatus[docType] === 'success' ? 0.7 : 1,
                        }}
                      >
                        {uploadStatus[docType] === 'uploading' ? 'Uploading...' : uploadStatus[docType] === 'success' ? '✓ Uploaded' : 'Upload'}
                      </button>
                    </div>
                  )}
                </div>
              ))}

              <div style={{ display: 'flex', gap: '1rem', marginTop: '2rem' }}>
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
                  onClick={handleSubmit}
                  disabled={loading || Object.values(uploadStatus).some(s => s !== 'success')}
                  style={{
                    flex: 1,
                    background: 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
                    color: 'white',
                    padding: '0.75rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: loading || Object.values(uploadStatus).some(s => s !== 'success') ? 'not-allowed' : 'pointer',
                    fontWeight: '600',
                    opacity: loading || Object.values(uploadStatus).some(s => s !== 'success') ? 0.5 : 1,
                  }}
                >
                  {loading ? 'Submitting...' : 'Submit Application'}
                </button>
              </div>



            </div>

          </div>
        )}
      </div>
    </div>
  );
}
