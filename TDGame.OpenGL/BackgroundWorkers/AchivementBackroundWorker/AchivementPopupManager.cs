using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.BackgroundWorkers;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.BackgroundWorkers.AchivementBackroundWorker
{
    public class AchivementPopupManager : BaseBackroundWorker
    {
        public override Guid ID { get; } = new Guid("50249fa6-d417-435e-be97-910940531f13");
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(2);
        public float Speed { get; set; } = 3;

        private enum MoveState { Hold, Up, Down }
        private MoveState _currentMoveState = MoveState.Hold;
        private TimeSpan _currentHoldTime = TimeSpan.Zero;
        private float _currentY = 1000f;
        private List<Guid> _previousAchivements = new List<Guid>();
        private AchivementControl _popup;

        public AchivementPopupManager(UIEngine parent, List<Guid> initialAchivements) : base(parent)
        {
            foreach (var id in initialAchivements)
                _previousAchivements.Add(id);
        }

        public override void Update(GameTime gameTime)
        {
            if (_popup == null)
            {
                var newItem = Parent.CurrentUser.Achivements.FirstOrDefault(x => !_previousAchivements.Contains(x));
                if (newItem != Guid.Empty)
                {
                    var achivement = ResourceManager.Achivements.GetResource(newItem);
                    _popup = new AchivementControl(Parent, achivement)
                    {
                        X = 0,
                        Y = _currentY,
                        FillColor = BasicTextures.GetBasicRectange(Color.Gray)
                    };
                    _popup.Initialize();
                    _currentMoveState = MoveState.Up;
                    _currentHoldTime = HoldTime;
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
                        if (_currentY <= 1000 - _popup._height)
                            _currentMoveState = MoveState.Hold;
                        break;
                    case MoveState.Down:
                        _currentY += Speed;
                        if (_currentY >= 1000)
                        {
                            _currentMoveState = MoveState.Hold;
                            _previousAchivements.Add(_popup.Achivement.ID);
                            _popup = null;
                        }
                        break;
                }
                if (_popup != null)
                {
                    _popup.Y = _currentY;
                    _popup.Update(gameTime);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_popup != null)
                _popup.Draw(gameTime, spriteBatch);
        }
    }
}
