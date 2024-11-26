import React, { useState, useCallback, useEffect } from 'react';
import { X, Info, Settings, HelpCircle, Play, Lock, Unlock, ArrowLeft, Star } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { motion, AnimatePresence } from 'framer-motion';

const SweetNebulaCrush = () => {
  const [currentScreen, setCurrentScreen] = useState('menu');
  const [unlockedLevels, setUnlockedLevels] = useState(1);
  const [currentLevel, setCurrentLevel] = useState(1);
  const [board, setBoard] = useState([]);
  const [selectedCandy, setSelectedCandy] = useState(null);
  const [score, setScore] = useState(0);
  const [moves, setMoves] = useState(0);

  const candyIcons = ['🍬', '🍭', '🍫', '🍰', '🍪', '🧁'];

  const levels = [
    { size: 5, target: 1000, moves: 15 },
    { size: 5, target: 1500, moves: 20 },
    { size: 6, target: 2000, moves: 20 },
    { size: 6, target: 2500, moves: 25 },
    { size: 7, target: 3000, moves: 25 },
    { size: 7, target: 3500, moves: 30 },
    { size: 8, target: 4000, moves: 30 },
    { size: 8, target: 4500, moves: 35 },
    { size: 9, target: 5000, moves: 35 },
    { size: 9, target: 5500, moves: 40 },
    { size: 10, target: 6000, moves: 40 },
    { size: 10, target: 6500, moves: 45 },
  ];

  const initializeBoard = useCallback((levelIndex) => {
    const level = levels[levelIndex] || levels[0];
    const size = level.size;
    const newBoard = [];
    for (let i = 0; i < size * size; i++) {
      newBoard.push(candyIcons[Math.floor(Math.random() * candyIcons.length)]);
    }
    setBoard(newBoard);
    setScore(0);
    setMoves(level.moves);
  }, [levels]);

  const checkForMatches = useCallback(() => {
    const level = levels[currentLevel - 1] || levels[0];
    const size = level.size;
    let matches = [];

    // Check horizontal matches
    for (let i = 0; i < size; i++) {
      for (let j = 0; j < size - 2; j++) {
        const index = i * size + j;
        if (
          board[index] === board[index + 1] &&
          board[index] === board[index + 2]
        ) {
          matches.push(index, index + 1, index + 2);
        }
      }
    }

    // Check vertical matches
    for (let i = 0; i < size - 2; i++) {
      for (let j = 0; j < size; j++) {
        const index = i * size + j;
        if (
          board[index] === board[index + size] &&
          board[index] === board[index + size * 2]
        ) {
          matches.push(index, index + size, index + size * 2);
        }
      }
    }

    return [...new Set(matches)];
  }, [board, currentLevel, levels]);

  const removeMatches = useCallback((matches) => {
    const level = levels[currentLevel - 1] || levels[0];
    const size = level.size;
    const newBoard = [...board];
    matches.forEach((index) => {
      for (let i = Math.floor(index / size); i > 0; i--) {
        newBoard[i * size + (index % size)] =
          newBoard[(i - 1) * size + (index % size)];
      }
      newBoard[index % size] = candyIcons[Math.floor(Math.random() * candyIcons.length)];
    });
    setBoard(newBoard);
    setScore((prevScore) => prevScore + matches.length * 10);
  }, [board, currentLevel, levels]);

  useEffect(() => {
    const matches = checkForMatches();
    if (matches.length > 0) {
      setTimeout(() => removeMatches(matches), 300);
    }
  }, [board, checkForMatches, removeMatches]);

  const handleCandyClick = (index) => {
    if (moves <= 0) return;

    if (selectedCandy === null) {
      setSelectedCandy(index);
    } else {
      const level = levels[currentLevel - 1] || levels[0];
      const size = level.size;
      const diff = Math.abs(selectedCandy - index);
      if (diff === 1 || diff === size) {
        const newBoard = [...board];
        [newBoard[selectedCandy], newBoard[index]] = [newBoard[index], newBoard[selectedCandy]];
        setBoard(newBoard);
        setMoves((prevMoves) => prevMoves - 1);
      }
      setSelectedCandy(null);
    }
  };

  const renderMenu = () => (
    <motion.div 
      initial={{ opacity: 0 }} 
      animate={{ opacity: 1 }} 
      exit={{ opacity: 0 }}
      className="flex flex-col items-center space-y-4"
    >
      <h1 className="text-4xl font-bold mb-6 text-purple-600">Sweet Nebula Crush</h1>
      <Button onClick={() => setCurrentScreen('levels')} className="w-48 bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600">
        <Play className="mr-2 h-4 w-4" /> Jugar
      </Button>
      <Dialog>
        <DialogTrigger asChild>
          <Button variant="outline" className="w-48">
            <Info className="mr-2 h-4 w-4" /> Información
          </Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Información del Juego</DialogTitle>
          </DialogHeader>
          <p>Sweet Nebula Crush ha sido creado por Claude.AI como parte práctica del treball de recerca de Marcos Cobos. Este juego demuestra la capacidad de la IA para crear experiencias interactivas y divertidas.</p>
        </DialogContent>
      </Dialog>
      <Button variant="outline" className="w-48 opacity-50 cursor-not-allowed" disabled>
        <Settings className="mr-2 h-4 w-4" /> Configuración
      </Button>
      <Dialog>
        <DialogTrigger asChild>
          <Button variant="outline" className="w-48">
            <HelpCircle className="mr-2 h-4 w-4" /> Tutorial
          </Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Cómo jugar</DialogTitle>
          </DialogHeader>
          <ol className="list-decimal list-inside space-y-2">
            <li>Haz clic en un caramelo y luego en un caramelo adyacente para intercambiarlos.</li>
            <li>Forma líneas de 3 o más caramelos iguales para eliminarlos y ganar puntos.</li>
            <li>Cada nivel tiene un objetivo de puntuación y un límite de movimientos.</li>
            <li>Completa el objetivo antes de quedarte sin movimientos para pasar al siguiente nivel.</li>
            <li>El tablero se hace más grande y los objetivos más difíciles a medida que avanzas.</li>
          </ol>
        </DialogContent>
      </Dialog>
      <Button variant="destructive" onClick={() => alert('¡Gracias por jugar!')} className="w-48">
        <X className="mr-2 h-4 w-4" /> Salir
      </Button>
    </motion.div>
  );

  const renderLevelSelector = () => (
    <motion.div 
      initial={{ opacity: 0 }} 
      animate={{ opacity: 1 }} 
      exit={{ opacity: 0 }}
      className="flex flex-col items-center"
    >
      <h2 className="text-2xl font-bold mb-4 text-purple-600">Selecciona un nivel</h2>
      <div className="grid grid-cols-3 gap-4">
        {levels.map((level, index) => (
          <Button
            key={index}
            onClick={() => {
              if (index < unlockedLevels) {
                setCurrentLevel(index + 1);
                initializeBoard(index);
                setCurrentScreen('game');
              }
            }}
            disabled={index >= unlockedLevels}
            className={`w-24 h-24 text-xl ${
              index < unlockedLevels
                ? 'bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600'
                : 'bg-gray-300'
            }`}
          >
            {index < unlockedLevels ? (
              <Unlock className="mr-2 h-4 w-4" />
            ) : (
              <Lock className="mr-2 h-4 w-4" />
            )}
            {index + 1}
          </Button>
        ))}
      </div>
      <Button onClick={() => setCurrentScreen('menu')} className="mt-4">
        Volver al Menú
      </Button>
    </motion.div>
  );

  const renderGame = () => (
    <motion.div 
      initial={{ opacity: 0 }} 
      animate={{ opacity: 1 }} 
      exit={{ opacity: 0 }}
      className="flex flex-col items-center"
    >
      <h2 className="text-2xl font-bold mb-4 text-purple-600">Nivel {currentLevel}</h2>
      <div className="flex justify-between w-full mb-2">
        <div className="text-xl font-bold">Puntuación: {score}/{(levels[currentLevel - 1] || levels[0]).target}</div>
        <div className="text-xl font-bold">Movimientos: {moves}</div>
      </div>
      <div className={`grid gap-1 bg-gradient-to-r from-purple-200 to-pink-200 p-2 rounded-lg grid-cols-${(levels[currentLevel - 1] || levels[0]).size}`}>
        <AnimatePresence>
          {board.map((candy, index) => (
            <motion.button
              key={index}
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              exit={{ scale: 0 }}
              transition={{ duration: 0.2 }}
              onClick={() => handleCandyClick(index)}
              className={`w-12 h-12 flex items-center justify-center bg-white rounded-full shadow-md hover:shadow-lg focus:outline-none focus:ring-2 focus:ring-purple-500 ${
                index === selectedCandy ? 'ring-2 ring-purple-500' : ''
              }`}
            >
              {candy}
            </motion.button>
          ))}
        </AnimatePresence>
      </div>
      <div className="flex space-x-4 mt-4">
        <Button
          onClick={() => {
            const level = levels[currentLevel - 1] || levels[0];
            if (score >= level.target) {
              if (currentLevel === unlockedLevels) {
                setUnlockedLevels(prev => Math.min(prev + 1, levels.length));
              }
              setCurrentScreen('levels');
            } else {
              alert('¡Aún no has alcanzado el objetivo de puntuación!');
            }
          }}
          className="bg-gradient-to-r from-green-500 to-blue-500 hover:from-green-600 hover:to-blue-600"
        >
          <Star className="mr-2 h-4 w-4" />
          {score >= (levels[currentLevel - 1] || levels[0]).target ? 'Completar Nivel' : 'Objetivo no alcanzado'}
        </Button>
        <Button variant="outline" onClick={() => setCurrentScreen('levels')}>
          <ArrowLeft className="mr-2 h-4 w-4" /> Volver a Niveles
        </Button>
      </div>
    </motion.div>
  );

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-r from-purple-400 via-pink-400 to-red-400">
      <div className="bg-white p-8 rounded-lg shadow-xl">
        <AnimatePresence mode="wait">
          {currentScreen === 'menu' && renderMenu()}
          {currentScreen === 'levels' && renderLevelSelector()}
          {currentScreen === 'game' && renderGame()}
        </AnimatePresence>
      </div>
    </div>
  );
};

export default SweetNebulaCrush;