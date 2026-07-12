#!/usr/bin/env bash
set -euo pipefail

REPO="${1:-akarakuts/lighthouses}"
LICENSE_FILE="${UNITY_LICENSE_FILE:-/Library/Application Support/Unity/Unity_lic.ulf}"

if ! command -v gh >/dev/null; then
  echo "Install GitHub CLI first: brew install gh && gh auth login"
  exit 1
fi

echo "Configuring Unity CI secrets for ${REPO}"
echo

if [[ -f "${LICENSE_FILE}" ]]; then
  gh secret set UNITY_LICENSE --repo "${REPO}" < "${LICENSE_FILE}"
  echo "✓ UNITY_LICENSE uploaded from ${LICENSE_FILE}"
else
  echo "⚠ License file not found at:"
  echo "  ${LICENSE_FILE}"
  echo
  echo "Activate a personal license in Unity Hub:"
  echo "  Unity Hub → Settings → Licenses → Add → Get a free personal license"
  echo
  read -r -p "Paste UNITY_LICENSE file contents manually? [y/N] " answer
  if [[ "${answer}" =~ ^[Yy]$ ]]; then
    gh secret set UNITY_LICENSE --repo "${REPO}"
    echo "✓ UNITY_LICENSE set"
  else
    echo "Skipping UNITY_LICENSE (Unity CI jobs will fail until it is set)."
  fi
fi

read -r -p "Unity account email (UNITY_EMAIL): " unity_email
if [[ -n "${unity_email}" ]]; then
  gh secret set UNITY_EMAIL --repo "${REPO}" --body "${unity_email}"
  echo "✓ UNITY_EMAIL set"
fi

read -r -s -p "Unity account password (UNITY_PASSWORD): " unity_password
echo
if [[ -n "${unity_password}" ]]; then
  gh secret set UNITY_PASSWORD --repo "${REPO}" --body "${unity_password}"
  echo "✓ UNITY_PASSWORD set"
fi

read -r -p "Unity serial for Pro/Plus (UNITY_SERIAL, optional): " unity_serial
if [[ -n "${unity_serial}" ]]; then
  gh secret set UNITY_SERIAL --repo "${REPO}" --body "${unity_serial}"
  echo "✓ UNITY_SERIAL set"
fi

echo
echo "Done. Re-run CI: gh workflow run \"Unity Quality\" --repo ${REPO} || git push"
