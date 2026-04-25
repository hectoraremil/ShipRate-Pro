import React from 'react';
import { Navbar as BsNavbar, Container } from 'react-bootstrap';
import { Box } from 'lucide-react';

const Navbar = () => {
  return (
    <BsNavbar sticky="top" className="mb-4">
      <Container>
        <BsNavbar.Brand href="#home" className="d-flex align-items-center gap-2">
          <div className="bg-primary p-2 rounded-3 d-flex align-items-center justify-content-center">
            <Box size={24} color="white" />
          </div>
          <span className="ms-1">ShipRate <span className="text-muted fw-light">Pro</span></span>
        </BsNavbar.Brand>
      </Container>
    </BsNavbar>
  );
};

export default Navbar;
