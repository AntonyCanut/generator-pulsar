'use strict';

var generators = require(`yeoman-generator`);
const yosay = require(`yosay`);

module.exports = class extends generators{
  constructor() {
    generators.Base.apply(this, arguments);
    this.argument('theName', {
      type: String,
      required: true
    });
  }
};


module.exports = class extends generators {
  initializing() {
    // Store all the values collected from the command line so we can pass to 
    // sub generators. I also use this to determine which data I still need to
    // prompt for.

    this.log(yosay(`Welcome to Pulsar Project powered by Antony Canut. Create your project with powerfull base.`));
 }

  prompting() {
    var _this = this;
    return this.prompt([
      {
        type: 'list',
        name: 'typeProject',
        message: 'What is your project ?',
        default: this.appname, // default to current folder name
        choices: [
            'Xamarin [Android - iOS]',
            'Xamarin Forms [Android - iOS]',
            // 'Kotlin [Android]',
            // 'Swift [iOS]'
        ],
        default: 'xamarin [Android - iOS]',
        store: true
      },
      ]).then(function(answers) {
          _this.props = answers;
        });
      }

    writing() {
      if (this.props.typeProject == 'Xamarin Forms [Android - iOS]') {
        this.composeWith('pulsar:xamarin-forms', { args: [this.theName] });
      }

      if (this.props.typeProject == 'Xamarin [Android - iOS]') {      
        this.composeWith('pulsar:xamarin', { args: [this.theName] });
      }

      // if (this.props.typeProject == 'Kotlin [Android]') {      
      //   this.composeWith('pulsar:kotlin', { args: [this.theName] });
      // }

      // if (this.props.typeProject == 'Swift [iOS]') {      
      //   this.composeWith('pulsar:swift', { args: [this.theName] });
      // }
    }
  };
