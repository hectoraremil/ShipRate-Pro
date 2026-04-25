import React from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { CheckCircle, Calendar } from 'lucide-react';

const QuoteResult = ({ quote }) => {
  if (!quote) return null;

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, scale: 0.95 }}
        className="glass-card p-4 mt-4 overflow-hidden position-relative"
      >
        <div className="position-absolute top-0 end-0 p-3">
          <CheckCircle size={32} className="text-success opacity-25" />
        </div>
        
        <h4 className="fw-bold mb-4 text-gradient">Cotización Generada</h4>
        
        <div className="row g-4">
          <div className="col-md-6 col-lg-4">
            <div className="p-3 rounded-4 bg-light bg-opacity-50 h-100">
              <p className="small text-muted text-uppercase fw-bold mb-1 d-flex align-items-center gap-1">
                <Calendar size={14} /> Fecha
              </p>
              <h5 className="mb-0 small">{new Date(quote.requestedAt).toLocaleDateString()}</h5>
            </div>
          </div>

          <div className="col-12 mt-4">
            <div className="p-4 rounded-4 bg-primary bg-opacity-10 border border-primary border-opacity-10">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <div>
                  <h6 className="text-primary fw-bold text-uppercase mb-1">{quote.countryName}</h6>
                  <p className="text-muted small mb-0">Tarifa: {quote.ratePerKilogram.toFixed(2)} USD/Kg</p>
                </div>
                <div className="text-end">
                  <span className="badge bg-primary rounded-pill px-3 py-2">{quote.weightKg} Kg</span>
                </div>
              </div>
              
              <hr className="my-3 opacity-10" />
              
              <div className="d-flex justify-content-between align-items-center mt-3">
                <span className="h5 mb-0 fw-bold">Total Estimado</span>
                <div className="text-end">
                  <span className="h3 mb-0 fw-bold text-primary">
                    {quote.totalAmount.toLocaleString('en-US', { style: 'currency', currency: quote.currencyCode })}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <p className="text-muted small mt-4 mb-0 text-center italic">
          * Los precios son estimados y pueden variar según dimensiones adicionales o seguros.
        </p>
      </motion.div>
    </AnimatePresence>
  );
};

export default QuoteResult;
