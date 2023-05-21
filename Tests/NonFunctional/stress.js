import http from 'k6/http';

import auth from './auth.js';
import { baseUrl, params, options as commonOptions } from './config.js';

export const options = Object.assign(commonOptions, {
  stages: [
    { duration: '15s', target: 50 },
    { duration: '2m', target: 130 },
    { duration: '3m', target: 130 },
    { duration: '15s', target: 0 }
  ],
  thresholds: {
    http_req_failed: ['rate<0.005'],
    http_req_duration: ['p(95)<1500']
  }
});

export default function () {
  auth();

  const res = http.post(
    `${baseUrl}/api/Attendance/passive`,
    JSON.stringify({
      stationId: 1,
      serialNumber: 1
    }),
    params
  );
}
