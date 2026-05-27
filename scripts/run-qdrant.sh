#!/usr/bin/env bash
set -e

QDRANT_VERSION="v1.11.5"
QDRANT_DIR="$HOME/.qdrant"
QDRANT_BIN="$QDRANT_DIR/qdrant"
QDRANT_URL="https://github.com/qdrant/qdrant/releases/download/${QDRANT_VERSION}/qdrant-x86_64-unknown-linux-musl.tar.gz"

mkdir -p "$QDRANT_DIR/storage"

if [ ! -f "$QDRANT_BIN" ]; then
  echo "Downloading Qdrant ${QDRANT_VERSION}..."
  curl -L "$QDRANT_URL" | tar -xz -C "$QDRANT_DIR"
  chmod +x "$QDRANT_BIN"
  echo "Done."
fi

# Start Ollama in background if not already running
if ! pgrep -x ollama > /dev/null 2>&1; then
  echo "Starting Ollama..."
  ollama serve > /tmp/ollama.log 2>&1 &
  sleep 2
fi

echo "Starting Qdrant on :6333 (gRPC :6334)..."
cd "$QDRANT_DIR"
exec ./qdrant
