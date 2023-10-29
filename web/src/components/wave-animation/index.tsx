import React, { useEffect, useRef } from 'react';

const WaveAnimation = () => {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const animationFrameRef = useRef<number | null>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;
    const centerY = height / 2;
    const amplitude = 10;
    const frequency = Math.random() * 0.45 + 0.05;
    const speed = 0.1;

    const drawWaves = (timestamp: number) => {
      ctx.clearRect(0, 0, width, height);

      for (let x = 0; x < width; x++) {
        const y = centerY + Math.sin((x * (frequency)) + speed * timestamp / 10) * amplitude;
        ctx.fillRect(x, y, 1, 1);
      }

      animationFrameRef.current = requestAnimationFrame(drawWaves);
    };

    animationFrameRef.current = requestAnimationFrame(drawWaves);

    return () => {
      if (animationFrameRef.current) {
        cancelAnimationFrame(animationFrameRef.current);
      }
    };
  }, []);

  return <canvas ref={canvasRef} width={200} height={20} />;
};

export default WaveAnimation;