import { Button, Card } from 'flowbite-react';

const NotFound = () => {
  return (
    <div className="h-screen flex items-center justify-center bg-gray-100">
      <Card className="text-center">
        <h1 className="text-6xl font-bold text-gray-800 mb-4">404</h1>
        <p className="text-2xl text-gray-600 mb-6">Page Not Found</p>
        <Button href="/" className="bg-blue-600 text-white py-3 px-6">
          Go to Login
        </Button>
      </Card>
    </div>
  );
};

export default NotFound;
