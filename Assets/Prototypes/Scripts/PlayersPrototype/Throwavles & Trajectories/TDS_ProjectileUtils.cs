using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_ProjectileUtils
{
    /// <summary>
    /// Get samed espaced points of a projectile motion's trajectory
    /// </summary>
    /// <param name="_originalPos">Original position of the projectile</param>
    /// <param name="_destinationPos">Destination position of the projectile</param>
    /// <param name="_velocity">Velocity of the projectile motion</param>
    /// <param name="_angle">Angle to launch (As Radians)</param>
    /// <param name="_posAmount">Amount of position to get (Minimum 3)</param>
    /// <returns>Returns the positions of the trajectory's points</returns>
    public static Vector3[] GetProjectileMotionPoints(Vector3 _originalPos, Vector3 _destinationPos, float _velocity, float _angle = 45, int _posAmount = 10)
    {
        // Clamp the angle value bewteen 0 & 360
        _angle = Mathf.Clamp(_angle, 0, 360);

        // Set the minimum position amount as 3
        if (_posAmount < 3) _posAmount = 3;

        // Create the array conaining the positions ; set the first as original position and the last as destination position
        Vector3[] _positions = new Vector3[_posAmount];
        _positions[0] = _originalPos;
        _positions[_posAmount - 1] = _destinationPos;

        // Get the distance between these positions in the X & Z axis plus on both
        float _distanceBetweenPosX = (_destinationPos.x - _originalPos.x) / _posAmount;
        float _distanceBetweenPosZ = (_destinationPos.z - _originalPos.z) / _posAmount;
        float _distanceBetweenPosXZ = Mathf.Sqrt(Mathf.Pow(_distanceBetweenPosX, 2) + Mathf.Pow(_distanceBetweenPosZ, 2));

        // Create float that will be used in the folowwing loop
        float _t = 0;
        float _x = 0;
        float _y = 0;
        float _z = 0;

        // Get the required position amount
        for (int _i = 1; _i < _posAmount - 1; _i++)
        {
            // Get the time value
            _t = (_distanceBetweenPosXZ * _i) / (_velocity * Mathf.Cos(_angle));

            // Get the X & Z value
            _x = _distanceBetweenPosX * _i;
            _z = _distanceBetweenPosZ * _i;

            // Get the Y value
            _y = -.5f * Physics.gravity.magnitude * Mathf.Pow(_t, 2) + _velocity * Mathf.Sin(_angle) * _t;

            // Add the point to the array
            _positions[_i] = new Vector3(_x, _y, _z);
        }

        // Return all positions
        return _positions;
    }

    /// <summary>
    /// Get the velocity of a projectile motion
    /// </summary>
    /// <param name="_originalPos">Original position of the projectile</param>
    /// <param name="_destinationPos">Destination position of the projectile</param>
    /// <param name="_angle">Angle to launch (As Radians)</param>
    /// <returns>Returns the velocity as float</returns>
    public static float GetProjectileVelocityAsFloat(Vector3 _originalPos, Vector3 _destinationPos, float _angle = 45)
    {
        // Clamp the angle value bewteen 0 & 360
        _angle = Mathf.Clamp(_angle, 0, 360);

        // Get the original & destination positions without Y axis value
        Vector3 _planeOriginPos = new Vector3(_originalPos.x, 0, _originalPos.z);
        Vector3 _planeDestPos = new Vector3(_destinationPos.x, 0, _destinationPos.z);

        // Get the distance between those two position
        float _distance = Vector3.Distance(_planeOriginPos, _planeDestPos);
        // Get the Y offset at start point
        float _yOffset = _originalPos.y - _destinationPos.y;

        // Calculate the initial velocity of the object as float
        float _velocity = (1 / Mathf.Cos(_angle)) * Mathf.Sqrt(.5f * Physics.gravity.magnitude * Mathf.Pow(_distance, 2) / (_distance * Mathf.Tan(_angle) + _yOffset));

        // Return the velocity
        return _velocity;
    }
    /// <summary>
    /// Get the velocity of a projectile motion
    /// </summary>
    /// <param name="_originalPos">Original position of the projectile</param>
    /// <param name="_destinationPos">Destination position of the projectile</param>
    /// <param name="_angle">Angle to launch (As Radians)</param>
    /// <returns>Returns the velocity as Vector3</returns>
    public static Vector3 GetProjectileVelocityAsVector3(Vector3 _originalPos, Vector3 _destinationPos, float _angle = 45)
    {
        // Clamp the angle value bewteen 0 & 360
        _angle = Mathf.Clamp(_angle, 0, 360);

        // Get the original & destination positions without Y axis value
        Vector3 _planeOriginPos = new Vector3(_originalPos.x, 0, _originalPos.z);
        Vector3 _planeDestPos = new Vector3(_destinationPos.x, 0, _destinationPos.z);

        // Get the distance between those two position
        float _distance = Vector3.Distance(_planeOriginPos, _planeDestPos);
        // Get the Y offset at start point
        float _yOffset = _originalPos.y - _destinationPos.y;

        // Calculate the initial velocity of the object as float
        float _initVelocity = (1 / Mathf.Cos(_angle)) * Mathf.Sqrt(.5f * Physics.gravity.magnitude * Mathf.Pow(_distance, 2) / (_distance * Mathf.Tan(_angle) + _yOffset));

        // Get the velocity of the object as Vector3
        Vector3 _velocity = new Vector3(0, _initVelocity * Mathf.Sin(_angle), _initVelocity * Mathf.Cos(_angle));

        // Calculate the angle between those two positions
        float _angleBetweenObjects = Vector3.Angle(Vector3.forward, _planeDestPos - _planeOriginPos);

        // If the destination is at a minus position on X axis than the start, set the angle as the opposate of itself
        if (_destinationPos.x < _originalPos.x)
        {
            _angleBetweenObjects = -_angleBetweenObjects;
        }

        // Get the velocity with the orientation as Vector3
        Vector3 _finalVelocity = Quaternion.AngleAxis(_angleBetweenObjects, Vector3.up) * _velocity;

        // Return the velocity with angle orientation
        return _finalVelocity;
    }
}
