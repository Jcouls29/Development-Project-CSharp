import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';

const BASE_URL = 'https://localhost:5001';
const PRODUCT_ID = 1;
const CATEGORY_ID = 1;

const errors = new Counter('errors');

export const options = {
  insecureSkipTLSVerify: true,
  stages: [
    { duration: '15s', target: 10  },  // ramp up to 10 users
    { duration: '30s', target: 50  },  // ramp up to 50 users
    { duration: '30s', target: 100 },  // ramp up to 100 users
    { duration: '30s', target: 150 },  // push to 150 users
    { duration: '15s', target: 0   },  // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'], // 95% of requests under 1000ms (wider window for higher load)
    http_req_failed:   ['rate<0.05'],  // less than 5% errors
    errors:            ['count<50'],   // fewer than 50 hard errors
  },
};

const JSON_HEADERS = { 'Content-Type': 'application/json' };

function addInventory() {
  const quantity = Math.floor(Math.random() * 10) + 1;
  const res = http.post(
    `${BASE_URL}/api/v1/inventory/add`,
    JSON.stringify({ productInstanceId: PRODUCT_ID, quantity: quantity }),
    { headers: JSON_HEADERS }
  );
  const ok = check(res, { 'add inventory 200': (r) => r.status === 200 });
  if (!ok) errors.add(1);
  return res;
}

function getCountByProduct() {
  const res = http.get(`${BASE_URL}/api/v1/inventory/count/${PRODUCT_ID}`);
  const ok = check(res, {
    'get count 200': (r) => r.status === 200,
    'count is a number': (r) => typeof r.json('count') === 'number',
  });
  if (!ok) errors.add(1);
  return res;
}

function searchByAttribute() {
  const res = http.post(
    `${BASE_URL}/api/v1/products/search`,
    JSON.stringify({ attributes: { Color: 'Red', Brand: 'Acme' } }),
    { headers: JSON_HEADERS }
  );
  const ok = check(res, {
    'search 200': (r) => r.status === 200,
    'returns array': (r) => Array.isArray(r.json()),
  });
  if (!ok) errors.add(1);
  return res;
}

function searchByCategory() {
  const res = http.post(
    `${BASE_URL}/api/v1/products/search`,
    JSON.stringify({ categoryIds: [CATEGORY_ID] }),
    { headers: JSON_HEADERS }
  );
  const ok = check(res, { 'search by category 200': (r) => r.status === 200 });
  if (!ok) errors.add(1);
  return res;
}

// Realistic mix: writes and reads
export default function () {
  const roll = Math.random();

  if (roll < 0.40) {
    addInventory();           // 40% — write heavy
  } else if (roll < 0.70) {
    getCountByProduct();      // 30% — simple read
  } else if (roll < 0.90) {
    searchByAttribute();      // 20% — read with JOINs
  } else {
    searchByCategory();       // 10% — read with JOIN
  }

  sleep(0.5); // 500ms think time between requests per VU
}
