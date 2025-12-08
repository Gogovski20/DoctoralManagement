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
import { useAuth } from './context/AuthContext';

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
      </Routes>
      
    </Router>
  );
}

export default App;