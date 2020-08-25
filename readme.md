## JSON Placeholder Challenge for OLO

### Goal
This repo is to display my ability to form a relatively straightforward and usable test framework. I am working with the API at https://jsonplaceholder.typicode.com/guide.html. This is a mock of a basic forum or chat like web service, with the ability to make posts and comments via different users. 

### Assumptions
The service itself doesn't actually persist data from post/put/patch/delete operations and doesn't actually have logins. I assume a real service would have to persist data, and I can't imagine such a service existing without some concept of a user session (especially since the API explicitely calls out UserIDs). As such I have written the tests with the assumption that data would persist and session and basic permissions would apply.

### Code Layout
The framework exists of 3 distinct layers, accessible via session objects.
1. The Functional Test Layer -- where the actual logic for testing occurs. This is the most complex layer, but also the uppermost with no dependencies. No test will affect the outcome of the other. Tests can call into the service layer via session objects. For unauthenticated sessions, a special handle exists to call.
2. The Service Layer -- where the service maps exist. Each service call is written as an extension on a session object. This is to ensure that the session object doesn't contain information that is not relevant to it, but is the entry point to calling any service. Since this is a very basic service, it is not very complicated -- It just routes named calls to an underlying communications layer.
3. The Communications Layer -- where actual network calls are made. This layer contains the logic to create and send network requests, setting the appropriate headers to ensure the call originates from the session it's called from and routing the call to the appropriate service.

In addition to the above three layers there are two other concepts to be aware of.
* Sessions -- The session is the basis of the framework. No service call can be made without first retrieving a session, either via authentication (which is mocked) or retrieving a special Unauthenticated or Corrupted session object.
* Object Models -- Services contain references to objects. Objects represent the types of data the API will typically handle. All objects should inherit the IEntity interface, ensuring that they can be serialized to http request content. In addition, there is a special inheritor of IEntity, the DirectStringContent object which can have it's value set directly -- useful for testing things like malformed json.

