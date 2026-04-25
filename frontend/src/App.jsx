import React, { useState } from 'react'
import { Container, Row, Col, Alert } from 'react-bootstrap'
import Navbar from './layouts/Navbar'
import Footer from './layouts/Footer'
import QuoteForm from './features/calculator/QuoteForm'
import QuoteResult from './features/calculator/QuoteResult'
import { calculateQuote } from './api/shippingService'
import { getUserFriendlyErrorMessage } from './api/apiError'
import { logger } from './utils/logger'
import { Truck } from 'lucide-react'

function App() {
  const [quote, setQuote] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleCalculate = async (weight, country) => {
    setLoading(true)
    setError(null)
    try {
      logger.info('Starting quote calculation from UI', { weight, country })
      const result = await calculateQuote(weight, country)
      setQuote(result)
    } catch (err) {
      logger.error('Quote calculation failed in UI', { error: err, weight, country })
      setError(getUserFriendlyErrorMessage(err, 'No fue posible calcular la tarifa en este momento.'))
      setQuote(null)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="app-wrapper">
      <Navbar />
      
      <main>
        <section className="hero-section">
          <Container>
            <Row className="justify-content-center align-items-center">
              <Col lg={10}>
                <div className="text-center mb-5">
                  <div className="d-inline-flex align-items-center justify-content-center p-3 mb-3 bg-primary bg-opacity-10 rounded-pill">
                    <Truck className="text-primary me-2" size={20} />
                    <span className="text-primary fw-bold small text-uppercase tracking-wider">Logística Global</span>
                  </div>
                  <h1 className="display-4 fw-bold mb-3">
                    Calcula tus <span className="text-gradient">Tarifas de Envío</span> en segundos
                  </h1>
                  <p className="lead text-muted mx-auto" style={{ maxWidth: '600px' }}>
                    Obtenga cotizaciones precisas basadas en el peso y destino de sus paquetes con nuestra herramienta profesional.
                  </p>
                </div>
              </Col>
            </Row>

            <Row className="justify-content-center">
              <Col lg={8}>
                {error && (
                  <Alert variant="danger" className="rounded-4 mb-4 border-0 shadow-sm d-flex align-items-center gap-2">
                    {error}
                  </Alert>
                )}
                
                <QuoteForm onCalculate={handleCalculate} loading={loading} />
                
                {quote && <QuoteResult quote={quote} />}
              </Col>
            </Row>
          </Container>
        </section>
      </main>

      <Footer />
    </div>
  )
}

export default App
