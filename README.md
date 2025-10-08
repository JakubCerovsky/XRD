# Library AR Navigator

This AR application helps you find a specific book in a school library. You scan a printed poster with a QR code. That single scan does two things:

1. Localizes the AR session (knows where you are) via image-tracking the poster background.
2. Picks the target book via the QR payload (e.g., isbn=...).

Then the app renders a path to the correct shelf.

![Sequence Diagram](./images/sequence_diagram.jpg)

### Use Cases

**Student**: _“Find this book now”_

- Scan the entrance poster’s QR for a specific ISBN; the app locks position and draws a floor path straight to the correct shelf segment.

**Librarian**: _“Where do I reshelve this?”_

- Scan the book’s barcode/ISBN, instantly get the shelf ID and a short route back to its exact bay.

### Video Demonstration

<!-- TODO: Input link -->

[Library AR Navigator Demo]()

### Contributors:

- [Jakub Cerovsky](https://github.com/JakubCerovsky)
- [Alexandro Bolfa](https://github.com/Reblayzer)
- [Cosmin Gabriel Demian](https://github.com/cosmindemian)
- [Marcus Cristofer Mitelea](https://github.com/mitmarcus)

### Tools Used

- [Unity 6](https://unity.com/)
  - AR Foundation, ARCore, ARKit XR Plugin, XR Management
- [3D Scanner App](https://3dscannerapp.com/)

<!-- TODO: Do we have some? -->

### Code References

The following resources were used during the project development process.

- [title](https://www.example.com)
