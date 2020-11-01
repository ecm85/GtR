import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render () {
        return (
            <form method='post' action='Gtr/GenerateImages'>
                <input type='submit' class='btn btn-primary' value='Generate Images' />
            </form>
        );
    }
}
