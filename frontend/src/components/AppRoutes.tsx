import { Routes, Route } from 'react-router-dom'
import TextCapture from './TextCapture'
import NotFound from './NotFound'

const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<TextCapture />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default AppRoutes;
