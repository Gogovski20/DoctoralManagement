// services/studentService.js - Updated
import apiClient from './apiClient';

export const studentService = {
  // Profile
  getStudentProfile: async () => {
    const response = await apiClient.get('/Auth/me');
    return response.data;
  },

  // Applications
  getApplications: async () => {
    const response = await apiClient.get('/Applications/my');
    return response.data;
  },

  getApplicationById: async (id) => {
    const response = await apiClient.get(`/Applications/${id}`);
    return response.data;
  },

  createApplicationDraft: async (data) => {
    const response = await apiClient.post('/Applications/create-draft', data);
    return response.data;
  },

  uploadApplicationDocument: async (applicationId, file, fileName, type) => {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('FileName', fileName || file.name);
    formData.append('Type', type);
  
    const response = await apiClient.post(
      `/Applications/${applicationId}/upload-document`,
      formData,
      {
        headers: { 
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },

  submitApplication: async (applicationId) => {
    const response = await apiClient.post(
      `/Applications/submit`,
      { applicationId: applicationId }
    );
    return response.data;
  },

  // Review application
  reviewApplication: async (id, reviewData) => {
    const response = await apiClient.put(`/Applications/${id}/review`, reviewData);
    return response.data;
  },


  // Doctoral Project
  getDoctoralProjects: async (studentId) => {
    const response = await apiClient.get(`/students/${studentId}/DoctoralProjects`);
    return response.data;
  },

  getDoctoralProjectById: async (projectId) => {
    const response = await apiClient.get(`/DoctoralProjects/${projectId}`);
    return response.data;
  },

  getMyDoctoralProjects: async () => {
    const response = await apiClient.get('/DoctoralProjects/my');
    return response.data;
  },

  createDoctoralProjectDraft: async (data) => {
    const payload = {
      studentId: data.studentId,
      mentorId: data.mentorId || data.preferredMentorId,
      title: data.title,
      researchArea: data.researchArea || data.description,
    };
  
    const response = await apiClient.post('/DoctoralProjects/create-draft', payload); 
    return response.data;
  },


  uploadDoctoralProjectProposal: async (projectId, file, documentType) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('fileName', file.name);
    formData.append('type', String(documentType)); // UploadDoctoralProjectProposalCommand expects this
    
    console.log('FormData contents:', {
      file: file.name,
      fileName: file.name,
      documentType: String(documentType), 
    });

    const response = await apiClient.post(`/DoctoralProjects/${projectId}/upload-proposal`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  submitDoctoralProject: async (projectId) => {
    // SubmitDoctoralProjectCommand expects projectId
    const response = await apiClient.post('/DoctoralProjects/submit', { 
      projectId: projectId
    });
    return response.data;
  },

  // Doctoral Projects - Admin/Mentor endpoints
  getAllDoctoralProjects: async () => {
    const response = await apiClient.get('/DoctoralProjects/all');
    return response.data;
  },

  getDoctoralProjectsByMentor: async (mentorId) => {
    const response = await apiClient.get(`/DoctoralProjects/by-mentor/${mentorId}`);
    return response.data;
  },

  reviewDoctoralProject: async (projectId, reviewData) => {
    const response = await apiClient.post('/DoctoralProjects/review', {
      projectId: projectId,
      newStatus: reviewData.newStatus,
      committeeNotes: reviewData.committeeNotes || '',
      documentStatus: reviewData.documentStatus,
      reviewComment: reviewData.reviewComment,
    });
    return response.data;
  },

  completeDoctoralProject: async (projectId, finalReportNotes) => {
    const response = await apiClient.post(`/DoctoralProjects/${projectId}/complete`, {
      finalReportNotes: finalReportNotes || '',
    });
    return response.data;
  },


  // Courses
  getCourses: async () => {
    const response = await apiClient.get('/Courses');
    return response.data;
  },

  getCourseById: async (id) => {
    const response = await apiClient.get(`/Courses/${id}`);
    return response.data;
  },

  createCourse: async (courseData) => {
    const response = await apiClient.post('/Courses', courseData);
    return response.data;
  },

  updateCourse: async (id, courseData) => {
    const response = await apiClient.put(`/Courses/${id}`, courseData);
    return response.data;
  },

  deleteCourse: async (id) => {
    const response = await apiClient.delete(`/Courses/${id}`);
    return response.data;
  },

  getCoursesBySemester: async (semester) => {
    const response = await apiClient.get(`/Courses/semester/${semester}`);
    return response.data;
  },

  enrollCourse: async (courseId) => {
    const response = await apiClient.post(`/Courses/${courseId}/enroll`);
    return response.data;
  },

  // Activities
  addPublication: async (formData) => {
    const response = await apiClient.post('/Publications', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  addMobility: async (formData) => {
    const response = await apiClient.post('/Mobilities', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  addConferenceParticipation: async (formData) => {
    const response = await apiClient.post('/ConferenceParticipations', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  // Admin endpoints
  createStudent: async (studentData) => {
    const response = await apiClient.post('/Students', studentData);
    return response.data;
  },

  getAllStudents: async () => {
    const response = await apiClient.get('/Students');
    return response.data;
  },

  getStudentById: async (id) => {
    const response = await apiClient.get(`/Students/${id}`);
    return response.data;
  },

  updateStudent: async (id, studentData) => {
    const response = await apiClient.put(`/Students/${id}`, studentData);
    return response.data;
  },

  deleteStudent: async (id) => {
    const response = await apiClient.delete(`/Students/${id}`);
    return response.data;
  },

  getAllMentors: async () => {
    const response = await apiClient.get('/Mentors');
    return response.data;
  },

  getMentorById: async (id) => {
    const response = await apiClient.get(`/Mentors/${id}`);
    return response.data;
  },

  updateMentor: async (id, mentorData) => {
    const response = await apiClient.put(`/Mentors/${id}`, mentorData);
    return response.data;
  },

  deleteMentor: async (id) => {
    const response = await apiClient.delete(`/Mentors/${id}`);
    return response.data;
  },

  createProgram: async (programData) => {
    const response = await apiClient.post('/DoctoralPrograms', programData);
    return response.data;
  },

  createCourse: async (courseData) => {
    const response = await apiClient.post('/Courses', courseData);
    return response.data;
  },

  createMentor: async (mentorData) => {
    const response = await apiClient.post('/Mentors', mentorData);
    return response.data;
  },

  getAllPrograms: async () => {
    const response = await apiClient.get('/DoctoralPrograms');
    return response.data;
  },

  getProgramById: async (id) => {
    const response = await apiClient.get(`/DoctoralPrograms/${id}`);
    return response.data;
  },

  updateProgram: async (id, programData) => {
    const response = await apiClient.put(`/DoctoralPrograms/${id}`, programData);
    return response.data;
  },

  deleteProgram: async (id) => {
    const response = await apiClient.delete(`/DoctoralPrograms/${id}`);
    return response.data;
  },

  getAllApplications: async (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.status) params.append('status', filters.status);
    if (filters.programId) params.append('programId', filters.programId);
    if (filters.studentId) params.append('studentId', filters.studentId);
  
    const queryString = params.toString();
    const url = queryString ? `/Applications?${queryString}` : '/Applications';
    const response = await apiClient.get(url);
    return response.data;
  },

  // ECTS Tracking
  getEctsStatus: async () => {
    const response = await apiClient.get(`/ECTSTrackings/my/status`);
    return response.data;
  },

  getEctsDetailed: async () => {
    const response = await apiClient.get(`ECTSTrackings/my/detailed`);
    return response.data;
  },

  // Conference Participations
  addConferenceParticipation: async (data) => {
    // data is plain JS object: { studentId, conferenceName, date, role, isInternational }
    const response = await apiClient.post('/ConferenceParticipations', data);
    return response.data;
  },

  getMyConferences: async () => {
    const response = await apiClient.get('/ConferenceParticipations/my');
    return response.data;
  },

  uploadConferenceDocument: async (conferenceId, file, fileName, type) => {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('FileName', fileName || file.name);
    formData.append('Type', type); // 3 = ConferenceProof

    const response = await apiClient.post(
      `/ConferenceParticipations/${conferenceId}/upload-document`,
      formData,
      {
        headers: { 
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },

  getConferenceById: async (conferenceId) => {
    const response = await apiClient.get(`/ConferenceParticipations/${conferenceId}`);
    return response.data;
  },

  downloadConferenceDocument: async (filePath, fileName) => {
    const response = await apiClient.get(`/ConferenceParticipations/download/${filePath}`, {
      responseType: 'blob',
    });
    // Create a download link
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    link.parentElement.removeChild(link);
    window.URL.revokeObjectURL(url);
  },
};