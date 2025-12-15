import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import LoginForm from './components/LoginForm';
import RegisterForm from './components/RegisterForm';
import StudentDashboard from './components/student/StudentDashboard';
import AdminDashboard from './components/admin/AdminDashboard';
import CreateApplicationPage from './components/student/CreateApplicationPage';
import AdminAddStudentPage from './components/admin/AdminAddStudentPage';
import AdminCreateProgramPage from './components/admin/AdminCreateProgramPage';
import AdminCreateMentorPage from './components/admin/AdminCreateMentorPage';
import ApplicationDetailPage from './components/student/ApplicationDetailPage';
import UploadDocumentsPage from './components/student/UploadDocumentsPage';
import AdminApplicationsPage from './components/admin/AdminApplicationsPage';
import AdminStudentsPage from './components/admin/AdminStudentsPage';
import EditStudentPage from './components/admin/EditStudentPage';
import AdminProgramsPage from './components/admin/AdminProgramsPage';
import EditProgramPage from './components/admin/EditProgramPage';
import AdminMentorsPage from './components/admin/AdminMentorsPage';
import EditMentorPage from './components/admin/EditMentorPage';
import AdminCoursesPage from './components/admin/AdminCoursesPage';
import AddCoursePage from './components/admin/AddCoursePage';
import EditCoursePage from './components/admin/EditCoursePage';
import ReviewApplicationDetailPage from './components/admin/ReviewApplicationDetailPage';
import ECTSTrackingPage from './components/student/ECTSTrackingPage';
import DoctoralProjectPage from './components/student/DoctoralProjectPage';
import CreateDoctoralProjectPage from './components/student/CreateDoctoralProjectPage';
import DoctoralProjectDetailPage from './components/student/DoctoralProjectDetailPage';
import AllDoctoralProjectsPage from './components/admin/AllDoctoralProjectsPage';
import AdminReviewDoctoralProjectPage from './components/admin/AdminReviewDoctoralProjectPage';
import CompleteDoctoralProjectPage from './components/CompleteDoctoralProjectPage';
import ActivitiesPage from './components/activities/ActivitiesPage';
import ConferenceParticipationsPage from './components/activities/ConferenceParticipationsPage';
import PublicationsPage from './components/activities/PublicationsPage';
import MobilitiesPage from './components/activities/MobilitiesPage';
import AdminConferencesPage from './components/admin/AdminConferencesPage';
import AdminReviewConferencePage from './components/admin/AdminReviewConferencePage';
import AdminMobilitiesPage from './components/admin/AdminMobilitiesPage';
import AdminPublicationsPage from './components/admin/AdminPublicationsPage';
import AdminReviewMobilityPage from './components/admin/AdminReviewMobilityPage';
import AdminReviewPublicationPage from './components/admin/AdminReviewPublicationPage';
import { useAuth } from './context/AuthContext';
import AdminEnrollStudentPage from './components/admin/AdminEnrollStudentPage';
import AdminCourseEnrollmentsPage from './components/admin/AdminCourseEnrollmentsPage';
import StudentCoursesPage from './components/student/StudentCoursesPage';
import EditApplicationPage from './components/student/EditApplicationPage';
import AdminThesisDefenses from './components/admin/AdminThesisDefenses';
import AdminScheduleThesisDefense from './components/admin/AdminScheduleThesisDefense';
import StudentUploadThesisDocument from './components/student/StudentUploadThesisDocument';
import AdminDoctoralProjectPage from './components/admin/AdminDoctoralProjectPage';
import AdminReviewThesisDocumentPage from './components/admin/AdminReviewThesisDocumentPage';

function App() {
  const { user } = useAuth();
  const isAuthenticated = !!user;

  const getRole = () => {
    if (!user?.token) return 'Student';
    try {
      const parts = user.token.split('.');
      if (parts.length !== 3) return 'Student';
      const decoded = JSON.parse(atob(parts[1]));
      return decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Student';
    } catch (err) {
      return 'Student';
    }
  };

  const role = getRole();

  return (
    <Router>
      <Navbar />
      <Routes>
        <Route
          path="/login"
          element={isAuthenticated ? <Navigate to="/dashboard" /> : <LoginForm />}
        />
        <Route
          path="/register"
          element={isAuthenticated ? <Navigate to="/dashboard" /> : <RegisterForm />}
        />
        <Route
          path="/dashboard"
          element={
            isAuthenticated ? (
              role === 'Admin' ? <AdminDashboard /> : <StudentDashboard />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/"
          element={<Navigate to={isAuthenticated ? '/dashboard' : '/login'} />}
        />
        
        {/* Student Routes */}
        <Route
          path="/applications/new"
          element={
            isAuthenticated && role === 'Student' ? (
              <CreateApplicationPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/applications/:id"
          element={
            isAuthenticated && role === 'Student' ? (
              <ApplicationDetailPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/applications/:id/upload"
          element={
            isAuthenticated && role === 'Student' ? (
              <UploadDocumentsPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        
        {/* Admin Routes */}
        <Route
          path="/admin/students/new"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AdminAddStudentPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin/programs/new"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AdminCreateProgramPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin/mentors/new"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AdminCreateMentorPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route path="/admin/applications" element={<AdminApplicationsPage />} />
      {/* <Route path="/admin/applications/:id" element={<AdminApplicationDetailPage />} /> */}
        <Route path="/admin/students" element={<AdminStudentsPage />} />
        <Route path="/admin/students/:id/edit" element={<EditStudentPage />} />
        <Route path="/admin/programs" element={<AdminProgramsPage />} />
        <Route path="/admin/programs/:id/edit" element={<EditProgramPage />} />
        <Route path="/admin/mentors" element={<AdminMentorsPage />} />
        <Route path="/admin/mentors/:id/edit" element={<EditMentorPage />} />
        <Route path="/admin/courses" element={<AdminCoursesPage />} />
        <Route path="/admin/courses/new" element={<AddCoursePage />} />
        <Route path="/admin/courses/:id/edit" element={<EditCoursePage />} />
        <Route path="/admin/applications/:id/review" element={<ReviewApplicationDetailPage />} />
        <Route path="/ects-tracking" element={<ECTSTrackingPage />} />
        <Route
          path="/doctoral-project"
          element={
            isAuthenticated && role === 'Student' ? (
              <DoctoralProjectPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/doctoral-project/new"
          element={
            isAuthenticated && role === 'Student' ? (
              <CreateDoctoralProjectPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />      
        <Route
          path="/doctoral-project/:id"
          element={
            isAuthenticated && role === 'Student' ? (
              <DoctoralProjectDetailPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin/doctoral-projects"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AllDoctoralProjectsPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin/doctoral-projects/:id/review"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AdminReviewDoctoralProjectPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/admin/doctoral-projects/:id/complete"
          element={
            isAuthenticated && role === 'Admin' ? (
              <CompleteDoctoralProjectPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route path="/activities" element={<ActivitiesPage />} />
        <Route path="/activities/conferences" element={<ConferenceParticipationsPage />} />
        <Route path="/activities/publications" element={<PublicationsPage />} />
        <Route path="/activities/mobilities" element={<MobilitiesPage />} />
        <Route path="/admin/conferences" element={<AdminConferencesPage />} />
        <Route path="/admin/conferences/:conferenceId/review" element={<AdminReviewConferencePage />} />
        <Route path="/admin/mobilities" element={<AdminMobilitiesPage />} />
        <Route path="/admin/publications" element={<AdminPublicationsPage />} />
        <Route path="/admin/mobilities/:mobilityId/review" element={<AdminReviewMobilityPage />} />
        <Route path="/admin/publications/:publicationId/review" element={<AdminReviewPublicationPage />} />
        <Route path="/admin/courses/:courseId/enroll" element={<AdminEnrollStudentPage />} />
        <Route path="/admin/enrollments" element={<AdminCourseEnrollmentsPage />} />
        <Route 
          path="/courses" 
          element={
            isAuthenticated && role === 'Student' ? (
              <StudentCoursesPage />
            ) : (
              <Navigate to="/login" />
            )
          } 
        />
        <Route
          path="/applications/:id/edit"
          element={
            isAuthenticated && role === 'Student' ? (
              <EditApplicationPage />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route path="/admin/thesis-defenses" element={<AdminThesisDefenses />} />
        <Route
          path="/admin/thesis-defenses/schedule"
          element={
            isAuthenticated && role === 'Admin' ? (
              <AdminScheduleThesisDefense />
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route
          path="/doctoral-project/:projectId/upload-thesis"
          element={
            isAuthenticated && role === 'Student'
              ? <StudentUploadThesisDocument />
              : <Navigate to="/login" />
          }
        />
        <Route path="/admin/doctoral-projects/:id" element={<AdminDoctoralProjectPage />} />
        <Route path="/admin/thesis-review/:projectId/:documentId" element={<AdminReviewThesisDocumentPage />} />
      </Routes>
    </Router>
  );
}

export default App;