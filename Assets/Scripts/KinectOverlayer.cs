using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
	public GUITexture backgroundImage;
	public KinectWrapper.NuiSkeletonPositionIndex TrackedJointRight = KinectWrapper.NuiSkeletonPositionIndex.HandRight;
	public KinectWrapper.NuiSkeletonPositionIndex TrackedJointLeft = KinectWrapper.NuiSkeletonPositionIndex.HandLeft;
	public GameObject OverlayObjectRight;
	public GameObject OverlayObjectLeft;
	public float smoothFactor = 5f;
	
	public GUIText debugText;

	private float distanceToCamera = 10f;


	void Start()
	{
		if(OverlayObjectRight)
		{
			distanceToCamera = (OverlayObjectRight.transform.position - Camera.main.transform.position).magnitude;
		}
		else if(OverlayObjectLeft)
		{
			distanceToCamera = (OverlayObjectLeft.transform.position - Camera.main.transform.position).magnitude;
		}
	}
	
	void Update() 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
			if(backgroundImage && (backgroundImage.texture == null))
			{
				backgroundImage.texture = manager.GetUsersClrTex();
			}
				
			int iJointIndexRight = (int)TrackedJointRight;
			int iJointIndexLeft = (int)TrackedJointLeft;

			
			if(manager.IsUserDetected())
			{
				uint userId = manager.GetPlayer1ID();
				
				if(manager.IsJointTracked(userId, iJointIndexRight))
				{
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndexRight);

					if(posJoint != Vector3.zero)
					{
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
						
						float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
						float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;
						

						if(debugText)
						{
							debugText.GetComponent<GUIText>().text = "Tracked user ID: " + userId;
						}
						
						if(OverlayObjectRight)
						{
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
							OverlayObjectRight.transform.position = Vector3.Lerp(OverlayObjectRight.transform.position, vPosOverlay, smoothFactor * Time.unscaledDeltaTime * 10);
						}
					}
				}
				if(manager.IsJointTracked(userId, iJointIndexLeft))
				{
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndexLeft);

					if(posJoint != Vector3.zero)
					{
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
						
						float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
						float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;
						

						if(debugText)
						{
							debugText.GetComponent<GUIText>().text = "Tracked user ID: " + userId;
						}
						
						if(OverlayObjectLeft)
						{
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
							OverlayObjectLeft.transform.position = Vector3.Lerp(OverlayObjectLeft.transform.position, vPosOverlay, smoothFactor * Time.unscaledDeltaTime * 100);
						}
					}
				}				
				
			}
			
		}
	}
}
