import { BrowserRouter, Navigate, Route } from "react-router-dom";

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