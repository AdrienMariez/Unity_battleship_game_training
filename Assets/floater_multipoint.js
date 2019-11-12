var waterLevel : float = 0.0;
var floatHeight : float = 1.0;
var bounceDamp : float = 0.05;

var bPoints : Transform[];

function Start () {
	
	if (bPoints[0] == null) {
		bPoints[0].transform.position = transform.position;
	}
}

function FixedUpdate () {
	
	for (var i = 0; i < bPoints.length; i++) {
		
		var actionPoint = bPoints[i].transform.position;
		var forceFactor = (1f - ((actionPoint.y - waterLevel) / floatHeight)) / bPoints.length;
	
		if (forceFactor > 0f) {
			var uplift = -Physics.gravity * (forceFactor - rigidbody.velocity.y * ((bounceDamp / bPoints.length) * Time.deltaTime));
			rigidbody.AddForceAtPosition(uplift, actionPoint);
		}
	}
}

