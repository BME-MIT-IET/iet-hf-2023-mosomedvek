import http from 'k6/http';

import auth from './auth.js';
import { baseUrl, params, options as commonOptions } from './config.js';

export const options = Object.assign(commonOptions, {
  stages: [
    { duration: '2m30s', target: 25 },
    { duration: '30m', target: 25 },
    { duration: '2m30s', target: 0 }
  ],
  thresholds: {
    http_req_failed: ['rate<0.001'],
    http_req_duration: ['p(95)<500']
  }
});

export default function () {
  auth();

  http.post(
    `${baseUrl}/api/Attendance/passive`,
    JSON.stringify({
      stationId: 1,
      serialNumber: 1
    }),
    params
  );
}
