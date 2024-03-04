using BugDefender.OpenGL.Engine.BackgroundWorkers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.BackgroundWorkers.NotificationBackroundWorker
{
    public class NotificationBackroundWorker : BaseBackroundWorker
    {
        public override Guid ID { get; } = new Guid("50249fa6-d417-435e-be97-910940531f13");
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(4);
        public float Speed { get; set; } = 3;
        public List<INotificationHandle> Handles { get; set; } = new List<INotificationHandle>();
        public GameWindow Parent { get; set; }

        private enum MoveState { Hold, Up, Down }
        private MoveState _currentMoveState = MoveState.Hold;
        private TimeSpan _currentHoldTime = TimeSpan.Zero;
        private float _currentY = 1080f;
        private readonly List<NotificationItem> _notificationStack = new List<NotificationItem>();
        private NotificationControl? _notification;
        private readonly TimeSpan _waitTarget = TimeSpan.FromSeconds(3);
        private TimeSpan _waitingFor = TimeSpan.FromSeconds(0);

        public NotificationBackroundWorker(GameWindow parent)
        {
            Parent = parent;
        }

        public void AddManualNotification(string name, string description)
        {
            _notificationStack.Add(new NotificationItem("", new ManualDefinition(name, description), false));
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
                        FillColor = Parent.UIResources.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"))
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
                        if (_currentY <= 1080 - _notification.Height)
                            _currentMoveState = MoveState.Hold;
                        break;
                    case MoveState.Down:
                        _currentY += Speed;
                        if (_currentY >= 1080)
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

        public void Skip()
        {
            if (_notification != null)
            {
                _currentMoveState = MoveState.Down;
                _currentHoldTime = TimeSpan.Zero;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_notification != null)
                _notification.Draw(gameTime, spriteBatch);
        }
    }
}
