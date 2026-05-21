import { BrowserRouter, Navigate, Route } from "react-router-dom";
import {LoginPage} ;
import {DashboardPage} ;
import ProtectedRoute from './guards/ProtectedRoute';

export default function App(){
  return(
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
    
        <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<DashboardPage />} />
        </Route>
      
        <Route path="*" element={<Navigate to = "/login" replace />} />  
      </Routes>

    </BrowserRouter>
  )
}