import { BrowserRouter, Routes,Navigate, Route } from "react-router-dom";
import LoginPage from './auth/LoginPage';
import DashboardPage from './dashboard/DashboadPage';
import AppLayout from './layout/AppLayout';
import AdminRoute from "./guards/AdminRoute";
import ProtectedRoute from './guards/ProtectedRoute';
import RegisterPage from "./admin/RegisterUserPage";

export default function App(){
  return(
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage/>}/>
        <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout/>}>
            <Route path="/dashboard" element={<DashboardPage />} />
             <Route element={<AdminRoute />}>
              {/* <Route path="/admin/register-user" element={</ />} /> */}
            </Route>
        </Route>
        </Route>
      
        <Route path="*" element={<Navigate to = "/login" replace />} />  
      </Routes>

    </BrowserRouter>
  )
}