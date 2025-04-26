using System;
using System.Collections.Generic;
using Domain.Tower.Services;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Enemy.Services;
using TowerDefense.Domain.Tower.Entities;
using TowerDefense.Domain.Wave.Entities;
using TowerDefense.Domain.Wave.Services;

namespace TowerDefense.Domain.Core.Services
{
    public class GameManager
    {
        private readonly WaveService _waveService;
        private readonly EnemyService _enemyService;
        private readonly TowerService _towerService;
        
        private WaveEntity _currentWave;
        private readonly List<TowerEntity> _towers = new List<TowerEntity>();
        private readonly List<Position> _path;
        private float _gameTime;
        private float _lastSpawnTime;
        private Cost _playerGold;
        
        public GameManager(List<Position> path, Cost initialGold)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _playerGold = initialGold;
            
            _enemyService = new EnemyService();
            _waveService = new WaveService(_enemyService);
            _towerService = new TowerService();
            
            _gameTime = 0f;
            _lastSpawnTime = 0f;
        }
        
        public void StartGame()
        {
            // 开始第一波
            StartNextWave();
        }
        
        public void Update(float deltaTime)
        {
            _gameTime += deltaTime;
            
            // 更新当前波次
            UpdateCurrentWave(deltaTime);
            
            // 更新所有防御塔
            UpdateTowers(deltaTime);
            
            // 更新所有敌人
            UpdateEnemies(deltaTime);
            
            // 检查波次是否完成
            if (_currentWave != null && _currentWave.IsCompleted)
            {
                // 开始下一波
                StartNextWave();
            }
        }
        
        private void StartNextWave()
        {
            int waveNumber = _currentWave == null ? 1 : _currentWave.WaveNumber + 1;
            _currentWave = _waveService.CreateWave(waveNumber);
            _lastSpawnTime = _gameTime;
        }
        
        private void UpdateCurrentWave(float deltaTime)
        {
            if (_currentWave == null || _currentWave.IsCompleted)
                return;
                
            // 检查是否需要生成新敌人
            if (_gameTime - _lastSpawnTime >= _currentWave.SpawnInterval && _currentWave.CanSpawnEnemy())
            {
                SpawnEnemy();
                _lastSpawnTime = _gameTime;
            }
        }
        
        private void SpawnEnemy()
        {
            if (_currentWave == null || !_currentWave.CanSpawnEnemy() || _path.Count < 2)
                return;
                
            // 使用路径的起点作为生成位置
            Position startPosition = _path[0];
            var enemy = _waveService.SpawnEnemyForWave(_currentWave, startPosition);
            
            if (enemy != null)
            {
                // 敌人已经在WaveEntity中注册，这里不需要额外操作
            }
        }
        
        private void UpdateTowers(float deltaTime)
        {
            foreach (var tower in _towers)
            {
                // 检查防御塔是否可以攻击
                if (tower.CanAttack(_gameTime))
                {
                    // 查找范围内的敌人
                    var enemiesInRange = FindEnemiesInRange(tower);
                    
                    if (enemiesInRange.Count > 0)
                    {
                        // 攻击第一个敌人
                        var targetEnemy = enemiesInRange[0];
                        float damage = _towerService.CalculateTowerDamage(tower, tower.Position.DistanceTo(targetEnemy.Position));
                        targetEnemy.TakeDamage(damage);
                        tower.Attack(_gameTime);
                    }
                }
            }
        }
        
        private List<EnemyEntity> FindEnemiesInRange(TowerEntity tower)
        {
            var result = new List<EnemyEntity>();
            
            if (_currentWave == null)
                return result;
                
            foreach (var enemy in _currentWave.Enemies)
            {
                if (!enemy.IsDead && tower.IsInRange(enemy.Position))
                {
                    result.Add(enemy);
                }
            }
            
            return result;
        }
        
        private void UpdateEnemies(float deltaTime)
        {
            if (_currentWave == null)
                return;
                
            foreach (var enemy in _currentWave.Enemies)
            {
                if (enemy.IsDead)
                    continue;
                    
                // 计算敌人的下一个位置
                Position nextPosition = _enemyService.CalculateNextPosition(enemy, _path[1], deltaTime);
                float distance = enemy.Position.DistanceTo(nextPosition);
                
                // 移动敌人
                enemy.Move(nextPosition, distance);
                
                // 检查敌人是否到达终点
                if (enemy.HasReachedEnd(_enemyService.CalculatePathLength(_path)))
                {
                    // 敌人到达终点，游戏结束
                    // 这里可以添加游戏结束逻辑
                }
            }
        }
        
        public bool CanPlaceTower(Position position)
        {
            // 检查位置是否有效（不在路径上，不与其他防御塔重叠）
            // 这里简化处理，实际游戏中需要更复杂的检查
            return true;
        }
        
        public bool PlaceTower(TowerType type, Position position)
        {
            if (!CanPlaceTower(position))
                return false;
                
            // 创建防御塔
            var tower = _towerService.CreateTower(type, position);
            
            // 检查玩家是否有足够的金币
            if (_playerGold < tower.UpgradeCost)
                return false;
                
            // 扣除金币
            _playerGold = _playerGold - tower.UpgradeCost;
            
            // 添加防御塔
            _towers.Add(tower);
            
            return true;
        }
        
        public bool UpgradeTower(TowerEntity tower)
        {
            if (tower == null || !_towers.Contains(tower))
                return false;
                
            // 检查玩家是否有足够的金币
            if (_playerGold < tower.UpgradeCost)
                return false;
                
            // 扣除金币
            _playerGold = _playerGold - tower.UpgradeCost;
            
            // 升级防御塔
            tower.Upgrade();
            
            return true;
        }
    }
} 