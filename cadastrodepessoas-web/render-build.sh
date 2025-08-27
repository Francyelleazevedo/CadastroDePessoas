#!/bin/bash
# Script de build para Render.com
npm ci --only=production
npm run build
