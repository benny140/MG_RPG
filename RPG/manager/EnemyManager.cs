using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPG.Physical;

namespace RPG.Manager;

public class EnemyManager
{
    private List<Enemy> _enemies;
    private Texture2D _enemyTexture;
    private Player _player;
    private int _maxEnemies;
    private Random _random;
    private int _spawnAreaWidth;
    private int _spawnAreaHeight;

    public EnemyManager(
        Texture2D enemyTexture,
        Player player,
        int maxEnemies,
        int spawnAreaWidth,
        int spawnAreaHeight
    )
    {
        _enemyTexture = enemyTexture;
        _player = player;
        _maxEnemies = maxEnemies;
        _enemies = new List<Enemy>();
        _random = new Random();
        _spawnAreaWidth = spawnAreaWidth;
        _spawnAreaHeight = spawnAreaHeight;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        _enemies.Clear();
        for (int i = 0; i < _maxEnemies; i++)
        {
            Vector2 randomPosition = new Vector2(
                _random.Next(0, _spawnAreaWidth),
                _random.Next(0, _spawnAreaHeight)
            );

            _enemies.Add(new Enemy(_enemyTexture, randomPosition, _player, 10));
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }
}
