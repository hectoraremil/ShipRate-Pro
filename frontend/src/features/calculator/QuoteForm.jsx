import React, { useState, useEffect } from 'react'
import { Alert, Form, Button, Row, Col, InputGroup, Spinner } from 'react-bootstrap'
import { Globe, Weight, Send } from 'lucide-react'
import { getCountries } from '../../api/shippingService'
import { getUserFriendlyErrorMessage } from '../../api/apiError'
import { logger } from '../../utils/logger'

const QuoteForm = ({ onCalculate, loading }) => {
  const [countries, setCountries] = useState([]);
  const [formData, setFormData] = useState({
    countryCode: '',
    weightKg: '',
  })
  const [fetchingCountries, setFetchingCountries] = useState(true)
  const [countriesError, setCountriesError] = useState('')

  useEffect(() => {
    const fetchCountries = async () => {
      try {
        setCountriesError('')
        const data = await getCountries()
        setCountries(data)
      } catch (error) {
        logger.error('Failed to fetch countries', { error })
        setCountriesError(
          getUserFriendlyErrorMessage(error, 'No fue posible cargar los paises disponibles. Intente nuevamente.'),
        )
      } finally {
        setFetchingCountries(false)
      }
    }

    fetchCountries()
  }, [])

  const handleSubmit = (e) => {
    e.preventDefault()
    if (formData.countryCode && formData.weightKg) {
      onCalculate(formData.weightKg, formData.countryCode)
    }
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  return (
    <div className="glass-card p-4">
      <h3 className="mb-4 fw-bold">Calculadora de Envío</h3>
      {countriesError ? (
        <Alert variant="warning" className="rounded-4 mb-4 border-0 shadow-sm">
          {countriesError}
        </Alert>
      ) : null}
      <Form onSubmit={handleSubmit}>
        <Row className="g-4">
          <Col md={6}>
            <Form.Group controlId="countryCode">
              <Form.Label className="fw-semibold small text-uppercase text-muted">País de Destino</Form.Label>
              <InputGroup>
                <InputGroup.Text className="bg-transparent border-end-0">
                  <Globe size={18} className="text-primary" />
                </InputGroup.Text>
                <Form.Select
                  name="countryCode"
                  value={formData.countryCode}
                  onChange={handleChange}
                  required
                  className="border-start-0"
                  disabled={fetchingCountries || Boolean(countriesError)}
                >
                  <option value="">{fetchingCountries ? 'Cargando países...' : 'Seleccione un país'}</option>
                  {countries.map((country) => (
                    <option key={country.countryCode} value={country.countryCode}>
                      {country.countryName}
                    </option>
                  ))}
                </Form.Select>
              </InputGroup>
            </Form.Group>
          </Col>
          <Col md={6}>
            <Form.Group controlId="weightKg">
              <Form.Label className="fw-semibold small text-uppercase text-muted">Peso del Paquete (Kg)</Form.Label>
              <InputGroup>
                <InputGroup.Text className="bg-transparent border-end-0">
                  <Weight size={18} className="text-primary" />
                </InputGroup.Text>
                <Form.Control
                  type="number"
                  step="0.01"
                  min="0.01"
                  name="weightKg"
                  value={formData.weightKg}
                  onChange={handleChange}
                  placeholder="ej. 5.5"
                  required
                  className="border-start-0"
                />
              </InputGroup>
            </Form.Group>
          </Col>
          <Col xs={12}>
            <Button
              variant="primary"
              type="submit"
              className="w-100 py-3 mt-2 d-flex align-items-center justify-content-center gap-2"
              disabled={loading || fetchingCountries || Boolean(countriesError)}
            >
              {loading ? (
                <>
                  <Spinner animation="border" size="sm" /> Calculando...
                </>
              ) : (
                <>
                  <Send size={18} /> Calcular Tarifa
                </>
              )}
            </Button>
          </Col>
        </Row>
      </Form>
    </div>
  )
}

export default QuoteForm
