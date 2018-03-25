'use strict';
var Generator = require('yeoman-generator');
var uuid = require('uuid');
var ejs = require('ejs');

/**
 * Returns the parameter unchanged.
 * Used when a file is already indented with tabs.
 * @param {String} contents The file contents.
 * @returns {String} The new file contents.
 */
function noop(contents) {
    return contents;
}

/**
 * Converts tabs to spaces.
 * @param {String} contents The file contents.
 * @returns {String} The new file contents.
 */
function tabsToSpaces(contents) {
    return contents.replace(/\t/g, '    ');
}

/**
 * Renders the given ejs template in the given context.
 * @param {String} contents The ejs template.
 * @param {*} context The context available during template rendering.
 * @returns {String} The rendered content.
 */
function ejsProcessor(contents, context) {
    return ejs.render(contents, context);
}

/**
 * Builds the processor function.
 * First the file is converted to spaces if needed, then it is rendered with ejs.
 * @param {String} indentationCharacter The indentation character ('tabs' or 'spaces').
 * @param {*} context The context available during template rendering.
 * @returns {Function} The processor function.
 */
function buildProcessor(indentationCharacter, context) {
    var fn = indentationCharacter === 'tabs' ? noop : tabsToSpaces;
    return (contents) => ejsProcessor(fn(contents.toString()), context);
}

/**
 * Builds the copier function.
 * @param {*} fs The filesystem.
 * @param {*} context The context available during template rendering.
 * @param {String} indentationCharacter The indentation character ('tabs' or 'spaces').
 * @returns {Function} The copier function.
 */
function buildCopier(fs, context, indentationCharacter) {
    return (from, to) => fs.copy(from, to, {
        process: buildProcessor(indentationCharacter, context)
    });
}

module.exports = class extends Generator {
    prompting() {
        var _this = this;
        return this.prompt([
            {
                type: 'input',
                name: 'name',
                message: 'Your project name',
                default: this.appname, // default to current folder name
                store: true
            },
            {
                type: 'input',
                name: 'companyName',
                message: 'Company name (for AssemblyInfo.cs copyright fields)',
                store: true
            }
        ]).then(function(answers) {
            _this.props = answers;
        });
    }

    writing() {
        var name = this.props.name;
        var iOS = "iOS";
        var droid = "Droid";
        var toolkit = "masterPackage"
        var options = {
            name: name,
            companyName: this.props.companyName,
            portableUUID: uuid.v1().toUpperCase(),
            iOSUUID: uuid.v1().toUpperCase(),
            androidUUID: uuid.v1().toUpperCase(),
            solutionFilesUUID: uuid.v1().toUpperCase(),
        };

        var copyFn = buildCopier(this.fs, options, 'tabs');

        // copy .gitignore
        this.fs.copyTpl(
            this.templatePath('_gitignore'),
            this.destinationPath('.gitignore'),
            options);

        // copy .travis.yml
        this.fs.copyTpl(
            this.templatePath('_travis.yml'),
            this.destinationPath('.travis.yml'),
            options);

        // copy solution file
        this.fs.copyTpl(
            this.templatePath('MyApp.sln'),
            this.destinationPath(name + '.sln'),
            options);
// -------------------------------
        // copy MyApp *.cs files
        copyFn(
            this.templatePath('MyApp/**/*.cs'),
            this.destinationPath(name)
        );
        copyFn(
            this.templatePath('Droid/**/*.cs'),
            this.destinationPath(droid)
        );
        copyFn(
            this.templatePath('iOS/**/*.cs'),
            this.destinationPath(iOS)
        );

// -------------------------------

        // copy MyApp *.json files
        copyFn(
            this.templatePath('iOS/**/*.json'),
            this.destinationPath(iOS)
        );

// -------------------------------

        /// copy MyApp *.xaml/xml/axml/plist/storyboard files
        copyFn(
            this.templatePath('MyApp/**/*.xaml'),
            this.destinationPath(name)
        );
        copyFn(
            this.templatePath('Droid/**/*.xml'),
            this.destinationPath(droid)
        );
        this.fs.copy(
            this.templatePath('Droid/**/*.png'),
            this.destinationPath(droid)
        );
        copyFn(
            this.templatePath('Droid/**/*.axml'),
            this.destinationPath(droid)
        );
        copyFn(
            this.templatePath('iOS/**/*.plist'),
            this.destinationPath(iOS)
        );
        copyFn(
            this.templatePath('iOS/**/*.storyboard'),
            this.destinationPath(iOS)
        );
        copyFn(
            this.templatePath('MyApp/**/*.resx'),
            this.destinationPath(name)
        );

// -------------------------------
        

        // copy MyApp *.config files
        this.fs.copyTpl(
            this.templatePath('MyApp/**/*.config'),
            this.destinationPath(name),
            options);
        this.fs.copyTpl(
            this.templatePath('Droid/**/*.config'),
            this.destinationPath(droid),
            options);
        this.fs.copyTpl(
            this.templatePath('iOS/**/*.config'),
            this.destinationPath(iOS),
            options);

// -------------------------------

        // copy MyApp.csproj file
        this.fs.copyTpl(
            this.templatePath('MyApp/MyApp.csproj'),
            this.destinationPath(name + '/' + name + '.csproj'),
            options);
        this.fs.copyTpl(
            this.templatePath('Droid/MyApp.Droid.csproj'),
            this.destinationPath(droid + '/' + name+'.'+ droid + '.csproj'),
            options);
        this.fs.copyTpl(
            this.templatePath('iOS/MyApp.iOS.csproj'),
            this.destinationPath(iOS + '/' + name+'.'+ iOS + '.csproj'),
            options);

// -------------------------------

        // copy Toolkit

        this.fs.copy(
            this.templatePath('masterPackage/*.dll'),
            this.destinationPath(toolkit + '/'));
    }
};
