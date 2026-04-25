import { apiClient } from './apiClient'

function mapCountry(country) {
  return {
    countryCode: country.countryCode,
    countryName: country.countryName,
  }
}

function mapQuote(quote) {
  return {
    shipmentQuoteId: quote.shipmentQuoteId,
    countryCode: quote.countryCode,
    countryName: quote.countryName,
    weightKg: Number(quote.weightKg),
    ratePerKilogram: Number(quote.ratePerKilogram),
    totalAmount: Number(quote.totalAmount),
    currencyCode: quote.currencyCode,
    requestedAt: quote.requestedAt,
  }
}

export async function getCountries() {
  const countries = await apiClient.get('/countries')
  return countries.map(mapCountry)
}

export async function calculateQuote(weightKg, countryCode) {
  const quote = await apiClient.post('/shipment-quotes', {
    weightKg: Number(weightKg),
    countryCode: countryCode.trim(),
  })

  return mapQuote(quote)
}

export async function getQuoteById(id) {
  const quote = await apiClient.get(`/shipment-quotes/${id}`)
  return mapQuote(quote)
}
