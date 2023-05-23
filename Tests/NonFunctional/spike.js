import http from 'k6/http';

import auth from './auth.js';
import { baseUrl, params, options as commonOptions } from './config.js';

export const options = Object.assign(commonOptions, {
  stages: [
    { duration: '40s', target: 500 },
    { duration: '20s', target: 0 }
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<3000']
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
