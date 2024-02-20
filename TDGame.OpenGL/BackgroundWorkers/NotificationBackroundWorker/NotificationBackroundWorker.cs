using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TDGame.OpenGL.BackgroundWorkers.NotificationBackroundWorker;
using TDGame.OpenGL.Engine.BackgroundWorkers;

namespace TDGame.OpenGL.BackgroundWorkers.AchivementBackroundWorker
{
    public class NotificationBackroundWorker : BaseBackroundWorker
    {
        public override Guid ID { get; } = new Guid("50249fa6-d417-435e-be97-910940531f13");
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(2);
        public float Speed { get; set; } = 3;
        public List<INotificationHandle> Handles { get; set; } = new List<INotificationHandle>();

        private enum MoveState { Hold, Up, Down }
        private MoveState _currentMoveState = MoveState.Hold;
        private TimeSpan _currentHoldTime = TimeSpan.Zero;
        private float _currentY = 1000f;
        private List<NotificationItem> _notificationStack = new List<NotificationItem>();
        private NotificationControl _notification;
        private TimeSpan _waitTarget = TimeSpan.FromSeconds(1);
        private TimeSpan _waitingFor = TimeSpan.FromSeconds(1);

        public NotificationBackroundWorker(UIEngine parent) : base(parent)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_notification == null)
            {
                _waitingFor -= gameTime.ElapsedGameTime;
                if (_waitingFor >= TimeSpan.Zero)
                    return;
                _waitingFor = _waitTarget;

                foreach (var handle in Handles)
                {
                    var newNotification = handle.GetNewNotification();
                    if (newNotification != null)
                        _notificationStack.Add(newNotification);
                }

                if (_notificationStack.Count > 0)
                {
                    var newItem = _notificationStack[0];
                    _notification = new NotificationControl(Parent, newItem)
                    {
                        X = 0,
                        Y = _currentY,
                        FillColor = TextureManager.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"))
                    };
                    _notification.Initialize();
                    _currentMoveState = MoveState.Up;
                    _currentHoldTime = HoldTime;
                    _notificationStack.RemoveAt(0);
                }
            }
            else
            {
                switch (_currentMoveState)
                {
                    case MoveState.Hold:
                        _currentHoldTime -= gameTime.ElapsedGameTime;
                        if (_currentHoldTime <= TimeSpan.Zero)
                            _currentMoveState = MoveState.Down;
                        break;
                    case MoveState.Up:
                        _currentY -= Speed;
                        if (_currentY <= 1000 - _notification._height)
                            _currentMoveState = MoveState.Hold;
                        break;
                    case MoveState.Down:
                        _currentY += Speed;
                        if (_currentY >= 1000)
                        {
                            _currentMoveState = MoveState.Hold;
                            _notification = null;
                        }
                        break;
                }
                if (_notification != null)
                {
                    _notification.Y = _currentY;
                    _notification.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_notification != null)
                _notification.Draw(gameTime, spriteBatch);
        }
    }
}
