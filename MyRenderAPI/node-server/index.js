crequire('dotenv').config(); // לטעינת משתנים מקובץ .env
const express = require('express');
const axios = require('axios');

const app = express();
const port = process.env.PORT || 3000;

app.get('/services', async (req, res) => {
  try {
    const response = await axios.get('https://api.render.com/v1/services', {
      headers: {
        'Authorization': `Bearer ${process.env.RENDER_API_KEY}`
      }
    });
    res.json(response.data);
  } catch (error) {
    console.error(error);
    res.status(500).send('Error fetching services');
  }
});

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
