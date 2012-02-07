using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace JumpTest
{
    public class Animation
    {
        public Object AnimatedObject;
        public string AnimatedProperty;

        private float mStartTime;
        private float mDuration;
        private float mTotalTime;
        private float mAnimationTime;
        private bool mLooping;
        private int mDirection;
        private bool mAnimationCompleted;
        private AnimationManager mParent;
        private AnimationEndCallback mEndAction;

        protected bool mIsRunning;

        public delegate void AnimationEndCallback();

        public Animation(float startTime,
                         float duration,
                         bool looping,
                         Object animated,
                         String property)
        {
            mStartTime = startTime;
            mDuration = duration;
            mLooping = looping;
            mParent = AnimationManager.GetInstance();
            mDirection = 1;
            mTotalTime = 0.0f;
            mAnimationTime = 0.0f;
            mIsRunning = false;
            AnimatedObject = animated;
            AnimatedProperty = property;
            mParent.Add(this);
            mAnimationCompleted = false;
            mEndAction = null;
        }

        public Animation(float startTime,
                         float duration,
                         bool looping,
                         Object animated,
                         String property,
                         AnimationEndCallback endingCallback)
        {
            mStartTime = startTime;
            mDuration = duration;
            mLooping = looping;
            mParent = AnimationManager.GetInstance();
            mDirection = 1;
            mTotalTime = 0.0f;
            mAnimationTime = 0.0f;
            mIsRunning = false;
            AnimatedObject = animated;
            AnimatedProperty = property;
            mParent.Add(this);
            mEndAction = endingCallback;
            mAnimationCompleted = false;
        }

        public void Stop()
        {
            mParent.Remove(this);
            mIsRunning = false;
            mAnimationCompleted = true;
        }

        public virtual void Update(float ElapsedTime)
        {

            float oldTotalTime = mTotalTime;
            mTotalTime += ElapsedTime;
            if (mTotalTime < mStartTime || mAnimationCompleted)
            {
                return;
            }
            if (!mIsRunning)
            {
                mIsRunning = true;
                mAnimationTime = mTotalTime - mStartTime;
            }
            else
            {
                mAnimationTime += ElapsedTime;
            }
            if (mAnimationTime > mDuration && !mLooping)
            {
                mParent.Remove(this);
                SetValue(1);
                mIsRunning = false;
                mAnimationCompleted = true;
                if (mEndAction != null)
                    mParent.AddAction(mEndAction);
                return;
            }
            while (mAnimationTime > mDuration)
            {
                mAnimationTime -= mDuration;
                mDirection = mDirection * -1;
            }
            if (mDirection > 0)
                SetValue(mAnimationTime / mDuration);
            else
                SetValue(1 - mAnimationTime / mDuration); 
        }

        public virtual void SetValue(float time)
        {

        }
    }


    public class FloatAnimation : Animation
    {

        public delegate void Apply(float c);

        public FloatAnimation(float startTime,
                                    float duration,
                                     bool looping,
                                     float from,
                                     float to,
                                     Apply changeFunction,
                                     Object objectToAnimate,
                                     string propertyToAnimate)
            : base(startTime, duration, looping, objectToAnimate, propertyToAnimate )
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }
        public FloatAnimation(float startTime,
                            float duration,
                             bool looping,
                             float from,
                             float to,
                             Apply changeFunction,
                             Object objectToAnimate,
                             string propertyToAnimate,
                             AnimationEndCallback endCallback)
            : base(startTime, duration, looping, objectToAnimate, propertyToAnimate, endCallback)
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }


        public override void SetValue(float percent)
        {            
            mChangeFunction(mFrom + (mTo - mFrom) * percent);
        }

        float mFrom;
        float mTo;
        Apply mChangeFunction;
    }




    public class Vector2Animation : Animation
    {

        public delegate void Apply(Vector2 v);

        public Vector2Animation(float startTime,
                                    float duration,
                                     bool looping,
                                     Vector2 from,
                                     Vector2 to,
                                     Apply changeFunction,
                                     Object objectToAnimate,
                                     string property)
            : base(startTime, duration, looping, objectToAnimate, property)
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }

        public Vector2Animation(float startTime,
                            float duration,
                             bool looping,
                             Vector2 from,
                             Vector2 to,
                             Apply changeFunction,
                             Object objectToAnimate,
                             string property,
                             AnimationEndCallback endCallback)
            : base(startTime, duration, looping, objectToAnimate, property, endCallback)
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }



        public override void SetValue(float percent)
        {
            Vector2 diffVector = mTo - mFrom;
            mChangeFunction((mTo - mFrom) * percent + mFrom);
        }

        Vector2 mFrom;
        Vector2 mTo;
        Apply mChangeFunction;
    }




    public class ColorAnimation : Animation
    {

        public delegate void Apply(Color c);

        public ColorAnimation(float startTime,
                                    float duration,
                                     bool looping,
                                     Color from,
                                     Color to,
                                     Apply changeFunction,
                                     Object objectToAnimate,
                                     string property)
            : base(startTime, duration, looping, objectToAnimate, property)
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }

        public ColorAnimation(float startTime,
                            float duration,
                             bool looping,
                             Color from,
                             Color to,
                             Apply changeFunction,
                             Object objectToAnimate,
                             string property,
                             AnimationEndCallback endCallback)
            : base(startTime, duration, looping, objectToAnimate, property, endCallback)
        {
            mFrom = from;
            mTo = to;
            mChangeFunction = changeFunction;
        }


       
        public override void SetValue(float percent)
        {
            mChangeFunction(Color.Lerp(mFrom, mTo, percent));
        }

        Color mFrom;
        Color mTo;
        Apply mChangeFunction;
    }

    public class AnimationManager
    {
        public static AnimationManager GetInstance()
        {
            if (mInstance == null)
                mInstance = new AnimationManager();
            return mInstance;
        }

        static AnimationManager mInstance = null;

        List<Animation> animations;
        List<Animation> animsToRemove;
        List<Animation.AnimationEndCallback> mActions;

        private AnimationManager()
        {
            animations = new List<Animation>();
            animsToRemove = new List<Animation>();
            mActions = new List<Animation.AnimationEndCallback>();
        }

        public void ClearAll()
        {
            animations.Clear();
        }

        public void AddAction(Animation.AnimationEndCallback action)
        {
            mActions.Add(action);
        }

        public void Add(Animation animToAdd)
        {
            foreach (Animation a in animations)
            {
                if (a.AnimatedObject == animToAdd.AnimatedObject && a.AnimatedProperty.Equals(animToAdd.AnimatedProperty))
                {
                    animsToRemove.Add(a);
                }
            }
            animations.Add(animToAdd);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Animation a in animsToRemove)
                animations.Remove(a);
            animsToRemove.Clear();
            foreach (Animation a in animations)
            {
                a.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            }
            foreach (Animation.AnimationEndCallback action in mActions)
            {
                action();
            }
            mActions.Clear();
        }
        public void Remove(Animation animToRemove)
        {
            animsToRemove.Add(animToRemove);
        }

    }
}
