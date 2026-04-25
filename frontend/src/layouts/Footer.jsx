import React from 'react';
import { Container } from 'react-bootstrap';

const Footer = () => {
  return (
    <footer className="footer text-center">
      <Container>
        <p className="text-muted mb-1 small">
          &copy; {new Date().getFullYear()} <strong>ShipRate Pro</strong>. Todos los derechos reservados.
        </p>
        <p className="text-muted extra-small" style={{ fontSize: '0.7rem' }}>
          Héctor Aremil Guzmán
        </p>
      </Container>
    </footer>
  );
};

export default Footer;
