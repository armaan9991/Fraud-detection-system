import { BrowserRouter, Routes,Navigate, Route } from "react-router-dom";
import LoginPage from './auth/LoginPage';
import DashboardPage from './dashboard/DashboadPage';
import AppLayout from './layout/AppLayout';

import ProtectedRoute from './guards/ProtectedRoute';

export default function App(){
  return(
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
    
        <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout/>}>
            <Route path="/dashboard" element={<DashboardPage />} />
        </Route>
        </Route>
      
        <Route path="*" element={<Navigate to = "/login" replace />} />  
      </Routes>

    </BrowserRouter>
  )
}